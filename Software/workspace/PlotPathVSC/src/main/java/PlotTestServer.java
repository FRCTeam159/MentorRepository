// PlotTestServer (host-based network tables server)
// Emulates a roborio (Robot) network server
// - On startup generates a new NetworkTable table named "datatable"
// - every 10 seconds publishes to the table an array object called NewPlot+id
// - fills plot data with random points (n-traces)
// 
// PlotTestClient
// - started first (before PlotTestServer is run)
// - waits for PlotTestSever to generate "datatable" server 
// - For every NewPlot+id that's published displays a java plotwindow that contains its data

import java.util.Random;
import edu.wpi.first.wpilibj.networktables.NetworkTable;

public class PlotTestServer {
	static int maxDataPoints = 100;

	public static void main(String [] args) {
		new PlotTestServer().run();
	}
	public void run(){
		NetworkTable table = NetworkTable.getTable("datatable");
		int traces=3;
		Random random = new Random();
		int maxScore = 10;
		double info[] = new double[4];
		int cnt=0;
		int id=0;
		String lpath=System.getProperty("java.library.path");
		System.out.println(lpath);
	
		while (true) {
			double tm=0;
			info[0]=cnt;//plot id
			info[1]=traces; // 1 trace
			info[2]=maxDataPoints;
			info[3]=PlotPath.DFLT_MODE;

			try {
				Thread.sleep(10000); // new plot every 10 seconds
			} catch (InterruptedException ex) {
				System.out.println("exception)");
			}
			table.putNumberArray("NewPlot"+id, info);
			System.out.println("NewPlot"+id);

			for (int i = 0; i < maxDataPoints; i++) {
				double data[] = new double[traces+2];

				data[0]=(double)i;
				data[1]=tm;
				for (int j = 0; j < traces; j++) {
					data[j+2]=(double) random.nextDouble() * maxScore;
				}
				table.putNumberArray("PlotData"+i, data);
				 //table.fireTableDataChanged();
				//table.flush();
				//System.out.println("server PlotData:" + i);

				tm+=0.02;
			}
			id++;
			cnt++;
		}
	}
}
