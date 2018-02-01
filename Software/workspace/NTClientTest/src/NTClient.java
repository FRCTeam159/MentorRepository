import edu.wpi.first.wpilibj.networktables.NetworkTable;
import edu.wpi.first.wpilibj.tables.ITable;
import edu.wpi.first.wpilibj.tables.ITableListener;

public class NTClient implements ITableListener {
	public static void main(String [] args) {
		new NTClient().run();
	}
	public void run(){
		NetworkTable.setClientMode();
		NetworkTable.setIPAddress("localhost");
		NetworkTable table = NetworkTable.getTable("datatable");
		
		table.addTableListener(this);
		//double x=0;
		//double y=0;
		while (true) {
			try {
				Thread.sleep(10000);
			} catch (InterruptedException ex) {
				System.out.println("exception)");
			}
//			x= table.getNumber("x", x);
//			y= table.getNumber("y", y);
//			System.out.println("x="+x+" y="+y);
		}
	}
	@Override
	public void valueChanged(ITable arg0, String arg1, Object arg2, boolean arg3) {
		// TODO Auto-generated method stub
		if(arg1.contentEquals("newdata")){
			System.out.println("newdata:"+arg2);
		}
		if(arg1.contentEquals("vals")){
			double vals[] = arg0.getNumberArray("vals", new double[0]);
			for(int i=0;i<vals.length;i++) {
				System.out.println("d["+i+"]="+vals[i]);

			}
		}
		if(arg1.contentEquals("enddata")){
			System.out.println("enddata:"+arg2);
		}

	}

}
