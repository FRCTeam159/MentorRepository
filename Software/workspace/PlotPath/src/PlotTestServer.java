
import java.util.Random;

import edu.wpi.first.wpilibj.networktables.NetworkTable;

public class PlotTestServer {
	static int maxDataPoints = 100;

	public static void main(String [] args) {
		new PlotTestServer().run();
	}
	public void run(){
		NetworkTable table = NetworkTable.getTable("datatable");
		int traces=6;
		Random random = new Random();
		int maxScore = 10;
		double info[] = new double[3];
		int cnt=0;
		while (true) {
			double tm=0;
			info[0]=cnt;//plot id
			info[1]=traces; // 1 trace
			info[2]=maxDataPoints;

			try {
				Thread.sleep(10000);
			} catch (InterruptedException ex) {
				System.out.println("exception)");
			}
			table.putNumberArray("NewPlot", info);

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
			
			cnt++;
		}
	}
}
