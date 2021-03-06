/*
 * CANTalon.cpp
 *
 *  Adds partial support for simulated CANTalon motor controllers
 *  - Automatically instantiates a Quad Encoder when needed
 *    -- For simulation, encoder DIO channels are assigned from CAN id ./dio/(id*2)+1/(id*2)+2
 *  - Automatically instantiates Forward and Reverse (hard) limit switches when needed
 *    -- Simulated using DIO channels based on CAN id rev=./dio/(id*2)+1 fwd=./dio/(id*2)+2
 *  - Instantiates up to 2 independent PIDContollers
 *    -- PIDControllers are instantiated on first call to SetPID (with pid_channel also set)
 */
#include "WPILib.h"
#include "CANTalon.h"

#define ID1 ((id-1)*2+1)
#define ID2 ((id-1)*2+2)

CANTalon::CANTalon(int i) :
        id(i), m_safetyHelper(new MotorSafetyHelper(this)) {
    char buf[64];
    for (int i = 0; i < MAXPIDCHNLS; i++) {
        pid_data[i].pid = 0;
    }
    limit_mode = kLimitMode_SrxDisableSwitchInputs;
    feedback_device = UnsetFeedbackDevice;
    encoder = 0;
    lowerLimit = 0;
    upperLimit = 0;

    debug = 0;
    control_mode = kPercentVbus;
    inverted = false;
    LiveWindow::GetInstance()->AddActuator("CANTalon", GetChannel(), this);
    // note: when can devices are supported in SW exporter can change this to "can/%d"
    sprintf(buf, "pwm/%d", i);
    impl = new SimContinuousOutput(buf);
}

CANTalon::~CANTalon() {
    ClearPID();
    if (encoder)
        delete encoder;
    if (lowerLimit)
        delete lowerLimit;
    if (upperLimit)
        delete upperLimit;
    if (m_table != nullptr)
        m_table->RemoveTableListener(this);
}

void CANTalon::SetDebug(int b) {
    debug = b;
}

double CANTalon::GetTargetError() {
    return pid_data[pid_channel].GetTargetError();
}

//======== Talon functions ===================
void CANTalon::Set(double val) {
	switch (control_mode) {
	default:
	case kFollower:
		break;
	case kPercentVbus:
		val=inverted?-val:val;
		impl->Set(val);
		break;
	case kSpeed:
		val=inverted?-val:val;
		SetSetpoint(val);
		break;
	case kPosition:
		SetSetpoint(val);
		break;
	}
   // SetVoltage(speed);
}
double CANTalon::Get() const{
	double val=impl->Get();
	switch (control_mode) {
	default:
	case kFollower:
		return 0;
		break;
	case kPercentVbus:
		val=impl->Get();
		break;
	case kSpeed:
		val=GetSpeed();
		break;
	case kPosition:
		val=GetPosition();
		break;
	}
    return val;
}
//========= Controller Interface functions ===================
void CANTalon::Enable() {
    EnableControl();
}
void CANTalon::Disable() {
    SetRaw(0);
    pid_data[pid_channel].Disable();
}

//========= MotorSafety Interface functions ===================

void CANTalon::SetExpiration(double timeout) {
    m_safetyHelper->SetExpiration(timeout);
}
double CANTalon::GetExpiration() const {
    return m_safetyHelper->GetExpiration();
}
bool CANTalon::IsAlive() const {
    return m_safetyHelper->IsAlive();
}

void CANTalon::StopMotor() {
    SetRaw(0);
}
void CANTalon::SetSafetyEnabled(bool enabled) {
    m_safetyHelper->SetSafetyEnabled(enabled);
}
bool CANTalon::IsSafetyEnabled() const {
    return m_safetyHelper->IsSafetyEnabled();
}
void CANTalon::GetDescription(std::ostringstream& desc) const {
    desc << "CAN " << GetChannel();
}
//========= PWM functions ===================
void CANTalon::SetVoltage(double val) {
    impl->Set(val);
    m_safetyHelper->Feed();
}
void CANTalon::SetRaw(unsigned short value) {
    //wpi_assert(value == 0);
    impl->Set(value);
}
//========== ITableListener Interface functions =========================
void CANTalon::ValueChanged(ITable* source, llvm::StringRef key,
        std::shared_ptr<nt::Value> value, bool isNew) {
    if (!value->IsDouble())
        return;
    SetVoltage(value->GetDouble());
}
std::string CANTalon::GetSmartDashboardType() const {
    return "Speed Controller";
}
std::shared_ptr<ITable> CANTalon::GetTable() const {
    return m_table;
}
void CANTalon::UpdateTable() {
    if (m_table != nullptr)
        m_table->PutNumber("Value", GetSpeed());
}
//========== LiveWindowSendable Interface functions ===========
void CANTalon::StartLiveWindowMode() {
    SetVoltage(0);
    if (m_table != nullptr)
        m_table->AddTableListener("Value", this, true);
}
void CANTalon::StopLiveWindowMode() {
    SetVoltage(0);
    if (m_table != nullptr)
        m_table->RemoveTableListener(this);
}
void CANTalon::InitTable(std::shared_ptr<ITable> subTable) {
    m_table = subTable;
    UpdateTable();
}
//=========== CANTalon PID control functions ==================
void CANTalon::SelectProfileSlot(int i) {
    pid_channel = i >= 1 ? 1 : 0;
}
void CANTalon::ClearPID() {
    for (int i = 0; i < MAXPIDCHNLS; i++) {
        pid_data[i].Clear();
    }
}
void CANTalon::SetP(double d) {
    pid_data[pid_channel].SetP(d);
}
void CANTalon::SetI(double d) {
    pid_data[pid_channel].SetI(d);
}
void CANTalon::SetD(double d) {
    pid_data[pid_channel].SetD(d);
}
void CANTalon::SetF(double d) {
    pid_data[pid_channel].SetF(d);
}
void CANTalon::SetPID(double P, double I, double D, double F) {
    pid_data[pid_channel].SetPID(P, I, D, F, this, this);
}
bool CANTalon::OnTarget() {
    return pid_data[pid_channel].OnTarget();
}
void CANTalon::ClearIaccum() {
    pid_data[pid_channel].Reset(); // clears accumulator but also disables
}
double CANTalon::GetF() {
    return pid_data[pid_channel].GetF();
}
//========= PIDInterface Interface functions =================
void CANTalon::SetPID(double P, double I, double D) {
    pid_data[pid_channel].SetPID(P, I, D, this, this);
}
double CANTalon::GetP() {
    return pid_data[pid_channel].GetP();
}
double CANTalon::GetI() {
    return pid_data[pid_channel].GetI();
}
double CANTalon::GetD() {
    return pid_data[pid_channel].GetD();
}
double CANTalon::GetSetpoint() {
    return pid_data[pid_channel].GetSetpoint();
}
void CANTalon::SetSetpoint(double value) {
    pid_data[pid_channel].SetSetpoint(value);
}
//========= PIDSource Interface function =====================
double CANTalon::PIDGet() {
    double d = ReturnPIDInput();
    if (IsEnabled() && (debug & 2))
        std::cout << id << " PIDGet:" << d << " setpoint:" << GetSetpoint()
                << std::endl;
    return d;
}
//========= PIDOutput Interface function ========================
void CANTalon::PIDWrite(double output) {
    if (IsEnabled() && (debug & 1))
        std::cout << id << " PIDWrite: target:" << GetSetpoint() << " error:"
                << GetTargetError() << " correction:" << output << std::endl;
    SetVoltage(output);
    m_safetyHelper->Feed();
}

void CANTalon::SetControlMode(ControlMode m) {
    if (m >= UnsetControlMode) {
        std::cout << "ERROR mode unsupported in simulation:" << m << std::endl;
        return;
    }
    control_mode = m;
    if ((m != kPosition) && (m != kSpeed)) {
        ClearPID();
    } else {
        PIDSourceType pidMode =
                (control_mode == kPosition) ?
                        PIDSourceType::kDisplacement : PIDSourceType::kRate;
        SetPIDSourceType(pidMode);
        if (encoder)
            encoder->SetPIDSourceType(pidMode);
    }
}
bool CANTalon::IsModePID(ControlMode mode) {
    return (mode == kSpeed) || (mode == kPosition);
}

void CANTalon::SetFeedbackDevice(FeedbackDevice device) {
    if (device >= UnsetFeedbackDevice) {
        std::cout << "ERROR feedback device unsupported in simulation:"
                << device << std::endl;
        return;
    }
    feedback_device = device;
    AddEncoder();
}

void CANTalon::AddEncoder() {
    if (!encoder)
        encoder = new Encoder(ID1, ID2); // {1,2} {3,4} {5,6} ..
}
void CANTalon::AddLowerLimit() {
    if (!lowerLimit)
        lowerLimit = new Limit(ID1, false);
}
void CANTalon::AddUpperLimit() {
    if (!upperLimit)
        upperLimit = new Limit(ID2, true);
}

double CANTalon::ReturnPIDInput() {
    switch (control_mode) {
    case kPosition:
        return GetPosition();
    case kSpeed:
        return GetSpeed();
    default:
    case kPercentVbus:
        return GetOutputVoltage();
    }
}

void CANTalon::SetVelocity(double value) {
    pid_data[pid_channel].SetSetpoint(value);
    if (debug)
        std::cout << id << " CANTalon::SetVelocity:" << value << std::endl;
}
void CANTalon::SetPosition(double value) {
    if (debug)
        std::cout << id << " CANTalon::SetPosition:" << value << std::endl;
    pid_data[pid_channel].SetSetpoint(value);
}

// in simulation rate is in degrees/second
double CANTalon::GetSpeed() const {
    if (encoder)
        return encoder->GetRate();
    else
        return 0;
}

double CANTalon::GetPosition() const {
    if (encoder)
        return encoder->GetDistance();
    else {
        std::cout << "ERROR CANTalon::GetPosition() encoder=NULL" << std::endl;
        return 0;
    }
}

//void CANTalon::Set(double value) {
//    Set(value, 0);
//}
float CANTalon::GetOutputVoltage() {
    return (float)impl->Get();
}
float CANTalon::GetOutputCurrent() {
    return (float)(10*impl->Get());
}

void CANTalon::EnableControl() {
    if (IsModePID(control_mode))
        pid_data[pid_channel].Set(this, this);
    pid_data[pid_channel].Enable();
}

bool CANTalon::IsEnabled() {
    return pid_data[pid_channel].IsEnabled();
}

void CANTalon::SetInverted(bool t) {
    inverted = t;
    // This causes purposefully thrown exception in simulation
    // if(encoder)
    //	 encoder->SetReverseDirection(t);
}

double CANTalon::GetTargetCorrection() {
    if (pid_data[pid_channel].pid)
        return pid_data[pid_channel].pid->Get();
    else
        return 0;
}

void CANTalon::Reset() {
    if (encoder)
        encoder->Reset();
    pid_data[pid_channel].Reset();
}
void CANTalon::ConfigEncoderCodesPerRev(uint16_t codesPerRev) {
    AddEncoder();
    encoder->SetDistancePerPulse((double) (1.0 / codesPerRev));
}

/**
 * Athena: Configures the soft limit enable (wear leveled persistent memory).
 *   - Also sets the limit switch overrides.
 * Simulation: Currently supports hard and soft limits only
 *   - Hard limit switches also require a (simulated) DigitalInput channel
 *   - Soft limit switches also require a Encoder
 */
void CANTalon::ConfigLimitMode(LimitMode mode) {
    switch (mode) {
    case kLimitMode_SoftPositionLimits:
        AddEncoder();
        AddUpperLimit();
        AddLowerLimit();
        break;
    case kLimitMode_SwitchInputsOnly:
        if (lowerLimit)
            lowerLimit->SetSoftLimitEnabled(false);
        if (upperLimit)
            upperLimit->SetSoftLimitEnabled(false);
        break;
    case kLimitMode_SrxDisableSwitchInputs:
        if (lowerLimit)
            lowerLimit->Disable();
        if (upperLimit)
            upperLimit->Disable();
        break;
    }
    limit_mode = mode;
}
/**
 * Athena: Change the fwd limit switch setting to normally open or closed.
 * @param normallyOpen true for normally open.  false for normally closed.
 *
 * Simulation: This function is not directly supported
 *   - DigitalInput "Get" function will always return true if the joint is in a position that meets
 *     the Joint limit properties set in the Solidworks exporter (or .sdf file)
 *   - DigitalInput ids that emulate CAN limit switches are assigned as follows:
 *     DIO channel: reverse=(id-1)*2+1 forward=(id-1)*2+2 {1,2},{2,3} ...
 *   - note: Simulated DigitalInput ids for switches are the same as PWM ids for simulated encoders
 */
void CANTalon::ConfigRevLimitSwitchNormallyOpen(bool normallyOpen) {
    AddLowerLimit();
}
/**
 * API is the same as for ConfigRevLimitSwitchNormallyOpen
 */
void CANTalon::ConfigFwdLimitSwitchNormallyOpen(bool normallyOpen) {
    AddUpperLimit();
}
/**
 * Hard Limit: Return true if joint is in predefined range (using DIO channel)
 * Soft Limit: Return true if encoder <= preset position
 */
int CANTalon::IsRevLimitSwitchClosed() {
    switch (limit_mode) {
    case kLimitMode_SwitchInputsOnly:
        AddLowerLimit();
        return lowerLimit->AtHardlimit() ? 1 : 0;
    case kLimitMode_SoftPositionLimits:
        AddLowerLimit();
        if (lowerLimit->AtSoftlimit())
            return 1;
        return lowerLimit->AtHardlimit() ? 1 : 0;
    default:
    case kLimitMode_SrxDisableSwitchInputs:
        return 0;
    }
}
/**
 * Hard Limit: Return true if joint is in predefined range
 * Soft Limit: Return true if encoder >= preset position
 */
int CANTalon::IsFwdLimitSwitchClosed() {
    switch (limit_mode) {
    case kLimitMode_SwitchInputsOnly:
        AddUpperLimit();
        return upperLimit->AtHardlimit() ? 1 : 0;
    case kLimitMode_SoftPositionLimits:
        AddUpperLimit();
        if (upperLimit->AtSoftlimit())
            return 1;
        return upperLimit->AtHardlimit() ? 1 : 0;
    default:
    case kLimitMode_SrxDisableSwitchInputs:
        return 0;
    }
}
void CANTalon::DisableSoftPositionLimits() {
    ConfigForwardSoftLimitEnable(false);
    ConfigReverseSoftLimitEnable(false);
}

void CANTalon::ConfigForwardLimit(double forwardLimitPosition) {
    AddUpperLimit();
    AddEncoder();
    upperLimit->SetSoftLimit(encoder, forwardLimitPosition);
}

void CANTalon::ConfigReverseLimit(double reverseLimitPosition) {
    AddLowerLimit();
    AddEncoder();
    lowerLimit->SetSoftLimit(encoder, reverseLimitPosition);
}
void CANTalon::ConfigSoftPositionLimits(double forward, double reverse) {
    ConfigForwardLimit(forward);
    ConfigReverseLimit(reverse);
}

void CANTalon::ConfigForwardSoftLimitEnable(bool bForwardSoftLimitEn) {
    if (upperLimit)
        upperLimit->SetSoftLimitEnabled(bForwardSoftLimitEn);
}

void CANTalon::ConfigReverseSoftLimitEnable(bool bReverseSoftLimitEn) {
    if (lowerLimit)
        lowerLimit->SetSoftLimitEnabled(bReverseSoftLimitEn);
}

bool CANTalon::GetForwardLimitOK() {
    if (upperLimit)
        return upperLimit->IsLimitOK();
    return true;
}

bool CANTalon::GetReverseLimitOK() {
    if (lowerLimit)
        return lowerLimit->IsLimitOK();
    return true;
}

void CANTalon::SetSensorDirection(bool reverseSensor) {
    // TODO: need to reverse soft limit switches ?
}

int CANTalon::GetEncVel() {
    // TODO: return raw encoder ticks ?
    return 0;
}

void CANTalon::SetEncPosition(int int1) {
    // TODO: set raw encoder ticks ?
}

int CANTalon::GetClosedLoopError() const {
    // TODO: Use this to mimic PIDController GetError ?
    return 0;
}

void CANTalon::SetAllowableClosedLoopErr(uint32_t allowableCloseLoopError) {
    // TODO: Use this to mimic PIDController OnTarget ?
}

void CANTalon::ConfigNeutralMode(NeutralMode mode) {
    // nothing to do in simulation
}

//=========================  private CANTalon::PIDdate Subclass =================
CANTalon::PIDData::PIDData() {
    P = I = D = F = 0;
    changed = false;
    pid = 0;
}
CANTalon::PIDData::~PIDData() {
    if (pid)
        delete pid;
    pid = 0;
}

void CANTalon::PIDData::SetPID(double P, double I, double D, double F,
        PIDSource *s, PIDOutput *d) {
    SetF(F);
    SetPID(P, I, D, s, d);
}
void CANTalon::PIDData::SetPID(double P, double I, double D, PIDSource *s,
        PIDOutput *d) {
    SetP(P);
    SetI(I);
    SetD(D);
    Set(s, d);
}
void CANTalon::PIDData::Set(PIDSource *s, PIDOutput *d) {
    if (changed && pid)
        Clear();
    if (!pid)
        pid = new PIDController(P, I, D, F, s, d, SIMPIDRATE);
    changed = false;
}
void CANTalon::PIDData::Clear() {
    if (pid)
        delete pid;
    pid = 0;
}
void CANTalon::PIDData::SetP(double d) {
    if (d != P)
        changed = true;
    P = d;
}
void CANTalon::PIDData::SetI(double d) {
    if (d != I)
        changed = true;
    I = d;
}
void CANTalon::PIDData::SetD(double d) {
    if (d != D)
        changed = true;
    D = d;
}
void CANTalon::PIDData::SetF(double d) {
    if (d != F)
        changed = true;
    F = d;
}
void CANTalon::PIDData::SetSetpoint(double value) {
    if (pid)
        pid->SetSetpoint(value);
}
bool CANTalon::PIDData::IsEnabled() {
    if (pid)
        return pid->IsEnabled();
    else
        return false;
}
void CANTalon::PIDData::Enable() {
    if (pid)
        pid->Enable();
}
void CANTalon::PIDData::Disable() {
    if (pid)
        pid->Disable();
}
void CANTalon::PIDData::Reset() {
    if (pid)
        pid->Reset();
}
double CANTalon::PIDData::GetTargetError() {
    if (pid)
        return pid->GetError();
    else
        return 0;
}
double CANTalon::PIDData::GetSetpoint() {
    if (pid)
        return pid->GetSetpoint();
    else
        return 0;
}
bool CANTalon::PIDData::OnTarget() {
    if (pid)
        return pid->OnTarget();
    else
        return false;
}
double CANTalon::PIDData::GetP() {
    return P;
}
double CANTalon::PIDData::GetI() {
    return I;
}
double CANTalon::PIDData::GetD() {
    return D;
}
double CANTalon::PIDData::GetF() {
    return F;
}

//=========================  CANTalon::Limit  ==================

CANTalon::Limit::Limit(int i, bool isFwrd) {
    id = i;
    enc = 0;
    soft_limit = 0;
    forward = isFwrd;
    hard_limit_enabled = true;
    soft_limit_enabled = false;
    dio = new DigitalInput(id);
}

CANTalon::Limit::~Limit() {
    if (dio)
        delete dio;
    dio = 0;
}

void CANTalon::Limit::SetSoftLimit(Encoder* encoder, double value) {
    enc = encoder;
    soft_limit = value;
    soft_limit_enabled = true;
}

bool CANTalon::Limit::AtHardlimit() {
    if (!dio)
        return false;
    if (!hard_limit_enabled)
        return false;
    return dio->Get() > 0 ? true : false;
}

//#define TEST
bool CANTalon::Limit::AtSoftlimit() {
    if (!enc)
        return false;
    if (!soft_limit_enabled)
        return false;
    if (forward && enc->GetDistance() >= soft_limit) {
#ifdef TEST
        std::cout<<id<< ":At Forward Soft Limit:"<<enc->GetDistance()<<":"<<soft_limit<<std::endl;
#endif
        return true;
    }
    if (!forward && enc->GetDistance() <= soft_limit) {
#ifdef TEST
        std::cout<<id<< ":At Reverse Soft Limit"<<enc->GetDistance()<<":"<<soft_limit<<std::endl;
#endif
        return true;
    }
    return false;
}
bool CANTalon::Limit::IsLimitOK() {
    if (AtHardlimit() || AtSoftlimit())
        return false;
    return true;
}

void CANTalon::Limit::SetHardLimitEnabled(bool b) {
    hard_limit_enabled = b;
}

void CANTalon::Limit::SetSoftLimitEnabled(bool b) {
    soft_limit_enabled = b;

}
void CANTalon::Limit::Disable() {
    SetHardLimitEnabled(false);
    SetSoftLimitEnabled(false);
}

