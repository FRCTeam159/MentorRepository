package org.usfirst.frc.team159.robot;

import java.awt.BorderLayout;
import java.awt.Graphics;
import java.awt.Polygon;
import java.awt.Rectangle;
import java.util.ArrayList;
import java.util.List;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.RenderingHints;
import java.awt.Stroke;
import javax.swing.JFrame;
import javax.swing.JPanel;

import edu.wpi.first.wpilibj.networktables.NetworkTable;

//import javafx.scene.Scene;
//import javafx.scene.chart.NumberAxis;
//import javafx.scene.chart.ScatterChart;
//import javafx.scene.chart.XYChart;
//import javafx.stage.Stage;

//import org.usfirst.frc.team159.robot.TestPlot.DrawSine;

public class PlotPath extends JFrame {
  /**
   * 
   */
  private static final long serialVersionUID = 1L;
  protected ArrayList<PathData> list;
  private static final int PREF_W = 800;
  private static final int PREF_H = 650;
  protected double tmax = 0;
  protected double ymin = 1000;
  protected double ymax = -1000;
  protected int traces;
  static public Color colors[] = { Color.BLUE, Color.RED, Color.GREEN, Color.ORANGE, Color.DARK_GRAY, Color.GRAY };
  public static NetworkTable table = NetworkTable.getTable("datatable");
  static public int id = 0;

  public PlotPath(ArrayList<PathData> d, int n) {
    list = d;
    traces = n;
    setSize(PREF_W, PREF_H);
    setTitle("Path Plot");
    setLocationRelativeTo(null);
    getLimits();
    add(new DrawPlot(), BorderLayout.CENTER);
    pack();
    setVisible(true);

  }

  void getLimits() {
    for (PathData data : list) {
      tmax = data.tm > tmax ? data.tm : tmax;
      for (int i = 0; i < PathData.DATA_SIZE; i++) {
        ymax = data.d[i] > ymax ? data.d[i] : ymax;
        ymin = data.d[i] < ymin ? data.d[i] : ymin;
      }
    }
    // System.out.println("tmax="+tmax+" ymin="+ymin+" ymax="+ymax);
  }

  public static void publish(ArrayList<PathData> d, int traces) {
    double info[] = new double[3];
    int points = d.size();
    info[0] = id;
    info[1] = traces;
    info[2] = points;
    table.putNumberArray("NewPlot" + id, info);
    System.out.println("PlotData.publish plot_id=" + id + " points=" + points + " traces=" + traces);
    id++;
    for (int i = 0; i < points; i++) {
      PathData pd = d.get(i);
      double data[] = new double[traces + 2];

      data[0] = (double) i;
      data[1] = pd.tm;
      for (int j = 0; j < traces; j++) {
        data[j + 2] = pd.d[j];
      }
      table.putNumberArray("PlotData" + i, data);
    }
  }

  class DrawPlot extends JPanel {
    private int padding = 25;
    private int labelPadding = 25;
    private Color pointColor = new Color(100, 100, 100, 180);
    private Color gridColor = new Color(200, 200, 200, 200);
    private int pointWidth = 2;
    private int numberYDivisions = 10;
    private Stroke GRAPH_STROKE = new BasicStroke(2f);
    private Stroke DASHED = new BasicStroke(3f, BasicStroke.CAP_ROUND, BasicStroke.JOIN_ROUND, 3.0f,
        new float[] { 5, 5 }, 0.0f);

    public DrawPlot() {
      setPreferredSize(new Dimension(800, 600));
    }

    protected void paintComponent(Graphics g) {
      super.paintComponent(g);
      // Graphics2D g2 = (Graphics2D)g;
      Graphics2D g2 = (Graphics2D) g.create();
      g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

      double xScale = ((double) getWidth() - (2 * padding) - labelPadding) / tmax;
      double yScale = ((double) getHeight() - 2 * padding - labelPadding) / (ymax - ymin);

      System.out.println("w=" + getWidth() + " h=" + getHeight() + " xScale=" + xScale + " yscale=" + yScale);

      // draw white background
      g2.setColor(Color.WHITE);
      g2.fillRect(padding + labelPadding, padding, getWidth() - (2 * padding) - labelPadding,
          getHeight() - 2 * padding - labelPadding);
      g2.setColor(Color.BLACK);

      // g2.setStroke(GRAPH_STROKE);

      // create hatch marks and grid lines for y axis.
      for (int i = 0; i < numberYDivisions + 1; i++) {
        int x0 = padding + labelPadding;
        int x1 = pointWidth + padding + labelPadding;
        int y0 = getHeight()
            - ((i * (getHeight() - padding * 2 - labelPadding)) / numberYDivisions + padding + labelPadding);
        int y1 = y0;
        if (list.size() > 0) {
          g2.setColor(gridColor);
          g2.drawLine(padding + labelPadding + 1 + pointWidth, y0, getWidth() - padding, y1);
          g2.setColor(Color.BLACK);
          String yLabel = ((int) ((ymin + (ymax - ymin) * ((i * 1.0) / numberYDivisions)) * 100)) / 100.0 + "";
          FontMetrics metrics = g2.getFontMetrics();
          int labelWidth = metrics.stringWidth(yLabel);
          g2.drawString(yLabel, x0 - labelWidth - 5, y0 + (metrics.getHeight() / 2) - 3);
        }
        g2.drawLine(x0, y0, x1, y1);
      }

      // and for x axis
      double last_tick = 0;
      for (int i = 0; i < list.size(); i++) {
        double new_tick = Math.round(list.get(i).tm);
        if (new_tick > last_tick) {
          int x0 = i * (getWidth() - padding * 2 - labelPadding) / (list.size() - 1) + padding + labelPadding;
          int x1 = x0;
          int y0 = getHeight() - padding - labelPadding;
          int y1 = y0 - pointWidth;
          g2.setColor(gridColor);
          g2.drawLine(x0, getHeight() - padding - labelPadding - 1 - pointWidth, x1, padding);
          g2.setColor(Color.BLACK);
          String xLabel = Math.round(list.get(i).tm) + "";
          FontMetrics metrics = g2.getFontMetrics();
          int labelWidth = metrics.stringWidth(xLabel);
          g2.drawString(xLabel, x0 - labelWidth / 2, y0 + metrics.getHeight() + 3);
          last_tick = new_tick;
          g2.drawLine(x0, y0, x1, y1);
        }
      }

      // create x and y axes
      g2.drawLine(padding + labelPadding, getHeight() - padding - labelPadding, padding + labelPadding, padding);
      g2.drawLine(padding + labelPadding, getHeight() - padding - labelPadding, getWidth() - padding,
          getHeight() - padding - labelPadding);

      int[] xs = new int[list.size()];
      int[] ys = new int[list.size()];
      for (int i = 0; i < list.size(); i++) {
        xs[i] = (int) (list.get(i).tm * xScale + padding + labelPadding);
      }

      for (int j = 0; j < traces; j++) {
        g2.setColor(colors[j]);
        for (int i = 0; i < list.size(); i++) {
          ys[i] = (int) ((ymax - list.get(i).d[j]) * yScale + padding);
        }
        if ((j % 2) == 0)
          g2.setStroke(GRAPH_STROKE);
        else
          g2.setStroke(DASHED);
        g2.drawPolyline(xs, ys, list.size());
      }
      g2.dispose();

    }

  }
}
