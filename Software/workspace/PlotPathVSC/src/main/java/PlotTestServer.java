import edu.wpi.first.networktables.NetworkTable;
import edu.wpi.first.networktables.NetworkTableEntry;
import edu.wpi.first.networktables.NetworkTableInstance;
import java.util.Random;

public class PlotTestServer {
    public static void main(String[] args) {
        System.out.println("Starting plot server)");
        new PlotTestServer().run();
    }

    public void run() {
        int maxDataPoints = 100;
        int cnt=0;
        int id=0;
        int traces=3;
		Random random = new Random();
		int maxScore = 10;
        NetworkTableInstance inst = NetworkTableInstance.getDefault();
        NetworkTable table = inst.getTable("datatable");
        double info[] = new double[4];
        inst.startServer();
       
        while (true) {
            try {
                Thread.sleep(5000); // new plot every 5 seconds
            } catch (InterruptedException ex) {
                System.out.println("exception)");
                return;
            }
            double tm=0;
			info[0]=cnt;//plot id
			info[1]=traces; // 1 trace
			info[2]=maxDataPoints;
			info[3]=PlotPath.DFLT_MODE;
            NetworkTableEntry entry = table.getEntry("NewPlot");
            entry.setDouble(id);
            entry = table.getEntry("PlotParams"+id);
            entry.setDoubleArray(info);

			System.out.println("PlotTestServer NewPlot id="+id);

			for (int i = 0; i < maxDataPoints; i++) {
				double data[] = new double[traces+2];
				data[0]=(double)i;
				data[1]=tm;
				for (int j = 0; j < traces; j++) {
					data[j+2]=(double) random.nextDouble() * maxScore;
                }
                entry = table.getEntry("PlotData"+i);
				entry.setDoubleArray(data);
				tm+=0.02;
			}
			id++;
            cnt++;
            
        }
    }
}