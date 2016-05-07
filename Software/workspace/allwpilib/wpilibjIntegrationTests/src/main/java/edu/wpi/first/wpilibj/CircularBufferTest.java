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

import edu.wpi.first.wpilibj.CircularBuffer;
import edu.wpi.first.wpilibj.test.AbstractComsSetup;

public class CircularBufferTest extends AbstractComsSetup {
  private static final Logger logger = Logger.getLogger(CircularBufferTest.class.getName());
  private double[] values = {751.848, 766.366, 342.657, 234.252, 716.126,
                             132.344, 445.697, 22.727, 421.125, 799.913};
  private double[] pushFrontOut = {799.913, 421.125, 22.727, 445.697, 132.344,
                              716.126, 234.252, 342.657};
  private double[] pushBackOut = {342.657, 234.252, 716.126, 132.344, 445.697,
                                  22.727, 421.125, 799.913};

  @Test
  public void pushFrontTest() {
    CircularBuffer queue = new CircularBuffer(8);

    for (double value : values) {
      queue.pushFront(value);
    }

    for (int i = 0; i < pushFrontOut.length; i++) {
      assertEquals(pushFrontOut[i], queue.get(i), 0.00005);
    }
  }

  @Test
  public void pushBackTest() {
    CircularBuffer queue = new CircularBuffer(8);

    for (double value : values) {
      queue.pushBack(value);
    }

    for (int i = 0; i < pushBackOut.length; i++) {
      assertEquals(pushBackOut[i], queue.get(i), 0.00005);
    }
  }

  @Test
  public void pushPopTest() {
    CircularBuffer queue = new CircularBuffer(3);

    // Insert three elements into the buffer
    queue.pushBack(1.0);
    queue.pushBack(2.0);
    queue.pushBack(3.0);

    assertEquals(1.0, queue.get(0), 0.00005);
    assertEquals(2.0, queue.get(1), 0.00005);
    assertEquals(3.0, queue.get(2), 0.00005);

    /*
     * The buffer is full now, so pushing subsequent elements will overwrite the
     * front-most elements.
     */

    queue.pushBack(4.0); // Overwrite 1 with 4

    // The buffer now contains 2, 3, and 4
    assertEquals(2.0, queue.get(0), 0.00005);
    assertEquals(3.0, queue.get(1), 0.00005);
    assertEquals(4.0, queue.get(2), 0.00005);

    queue.pushBack(5.0); // Overwrite 2 with 5

    // The buffer now contains 3, 4, and 5
    assertEquals(3.0, queue.get(0), 0.00005);
    assertEquals(4.0, queue.get(1), 0.00005);
    assertEquals(5.0, queue.get(2), 0.00005);

    assertEquals(5.0, queue.popBack(), 0.00005); // 5 is removed

    // The buffer now contains 3 and 4
    assertEquals(3.0, queue.get(0), 0.00005);
    assertEquals(4.0, queue.get(1), 0.00005);

    assertEquals(3.0, queue.popFront(), 0.00005); // 3 is removed

    // Leaving only one element with value == 4
    assertEquals(4.0, queue.get(0), 0.00005);
  }

  @Override
  protected Logger getClassLogger() {
    return logger;
  }
}
