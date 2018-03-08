package org.usfirst.frc.team159.robot.subsystems;

import java.util.ArrayList;

import javax.swing.JFrame;
import javax.swing.SwingUtilities;

import org.usfirst.frc.team159.robot.PathData;
import org.usfirst.frc.team159.robot.PlotPath;

import edu.wpi.first.wpilibj.networktables.NetworkTable;
import edu.wpi.first.wpilibj.tables.ITable;
import edu.wpi.first.wpilibj.tables.ITableListener;

public class PlotNetClient implements ITableListener{
	ArrayList<PathData> list = new ArrayList<PathData>();
	int traces=0;
	int index=0;
	int points=0;
	int count=0;
	static int plot_count=0;
	static int plot_id=0;
	public static void main(String[] args) {
		new PlotNetClient().run();
	}

	public void run() {
		NetworkTable.setClientMode();
		NetworkTable.setIPAddress("localhost");
		NetworkTable table = NetworkTable.getTable("datatable");

		table.addTableListener(this,true);
		while (true) {
			try {
				Thread.sleep(1000);
			} catch (InterruptedException ex) {
				System.out.println("exception)");
			}
		}
	}

	private static void createAndShowGui(int id, ArrayList<PathData> list, int traces) {
		JFrame frame = new PlotPath(list, traces);
		System.out.println("Creating Plot "+id);
		//frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.pack();
		frame.setLocationRelativeTo(null);
		frame.setVisible(true);
	}

	@Override
	public void valueChanged(ITable arg0, String arg1, Object arg2, boolean arg3) {
		if (arg1.contentEquals("NewPlot"+plot_count)) {
			list.clear();
			double info[] = arg0.getNumberArray("NewPlot"+plot_count, new double[0]);
			plot_id = (int) info[0];
			traces = (int) info[1];
			points = (int) info[2];
			index=0;
			count=0;
			System.out.println("NewPlot:" + plot_id + " " + traces + " " + points);

		}
		if (arg1.contentEquals("PlotData"+count)) {
			double data[] = arg0.getNumberArray("PlotData"+count, new double[0]);
			PathData pd = new PathData();

			index = (int) data[0];
			pd.tm= data[1];
			for(int i=0;i<data.length-2;i++) {
				pd.d[i]=data[i+2];
			}
			list.add(pd);
			count++;
			//System.out.println("PlotData:" + index);

		}
		if((count==points) && (points>0) && (plot_id==plot_count)) {
			count=0;
			index=0;
			plot_count++;

			SwingUtilities.invokeLater(new Runnable() {
				public void run() {
					createAndShowGui(plot_id, list, traces);
				}
			});
		}
		
	}
}
