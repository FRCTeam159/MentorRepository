/*----------------------------------------------------------------------------*/
/* Copyright (c) FIRST 2008-2016. All Rights Reserved.                        */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package edu.wpi.first.wpilibj;

import static org.junit.Assert.assertEquals;

import java.util.logging.Logger;

import org.junit.Test;

import edu.wpi.first.wpilibj.test.AbstractComsSetup;

import edu.wpi.first.wpilibj.DigitalInput;
import edu.wpi.first.wpilibj.Encoder;
import edu.wpi.first.wpilibj.Counter;
import edu.wpi.first.wpilibj.DigitalGlitchFilter;

/**
 * Test for the DigitalGlitchFilter class.
 *
 * @author Austin Schuh and Jerry Morrison
 */
public class DigitalGlitchFilterTest extends AbstractComsSetup {
  private static final Logger logger = Logger.getLogger(
     DigitalGlitchFilterTest.class.getName());

  protected Logger getClassLogger() {
    return logger;
  }

  /**
   * This is a test to make sure that filters can be created and are consistent.
   * This assumes that the FPGA implementation to actually implement the filter
   * has been tested.  It does validate that we are successfully able to set and
   * get the active filters for ports and makes sure that the FPGA filter is
   * changed correctly, and does the same for the period.
   */
  @Test
  public void testDigitalGlitchFilterBasic() {
    DigitalInput input1 = new DigitalInput(1);
    DigitalInput input2 = new DigitalInput(2);
    DigitalInput input3 = new DigitalInput(3);
    DigitalInput input4 = new DigitalInput(4);
    Encoder encoder5 = new Encoder(5, 6);
    Counter counter7 = new Counter(7);

    DigitalGlitchFilter filter1 = new DigitalGlitchFilter();
    filter1.add(input1);
    filter1.setPeriodNanoSeconds(4200);

    DigitalGlitchFilter filter2 = new DigitalGlitchFilter();
    filter2.add(input2);
    filter2.add(input3);
    filter2.setPeriodNanoSeconds(97100);

    DigitalGlitchFilter filter3 = new DigitalGlitchFilter();
    filter3.add(input4);
    filter3.add(encoder5);
    filter3.add(counter7);
    filter3.setPeriodNanoSeconds(167800);

    assertEquals(4200, filter1.getPeriodNanoSeconds());
    assertEquals(97100, filter2.getPeriodNanoSeconds());
    assertEquals(167800, filter3.getPeriodNanoSeconds());

    filter1.remove(input1);

    filter2.remove(input2);
    filter2.remove(input3);

    filter3.remove(input4);
    filter3.remove(encoder5);
    filter3.remove(counter7);

    input1.free();
    input2.free();
    input3.free();
    input4.free();
    encoder5.free();
    counter7.free();
    filter1.free();
    filter2.free();
    filter3.free();
  }
}
