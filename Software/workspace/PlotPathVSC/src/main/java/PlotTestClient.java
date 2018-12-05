
// PlotTestClient
// plots data sent by a NetworkTable server
// data sources
//  1) fake data sent by running PlotTestServer or PathPlotTest main functions included in this project 
//   - ip=localhost (default if no program argument given in launch.json)
//  2) Pathfinder data generated on the Roborio as part of the Robot program
//     ip=10.1.59.2 (set "args": "10.1.59.2" in launch.json)
// - On startup looks at optional "ip" argument (set in launch.json)
// - Network client attaches to a server named "datatable"
// - waits for a new array string called NewPlot+id to appear in the table
// - decodes the number of points (npoints) and traces (ntraces)in the plot from the array data
// - waits for new entries named PlotData+i to appear in the table i=0 to npoints*ntraces
// - when all expected points have arrived and saved into a data buffer outputs a new java plot of the data
// - increments the plot id counter (id)
// - waits for another NewPlot+id message to appear in the table

import java.util.ArrayList;

import javax.swing.JFrame;
import javax.swing.SwingUtilities;

import edu.wpi.first.networktables.EntryListenerFlags;
import edu.wpi.first.networktables.NetworkTable;
import edu.wpi.first.networktables.NetworkTableEntry;
import edu.wpi.first.networktables.NetworkTableInstance;
import edu.wpi.first.networktables.NetworkTableValue;
import edu.wpi.first.networktables.TableEntryListener;

public class PlotTestClient implements TableEntryListener, EntryListenerFlags {
    ArrayList<PathData> list = new ArrayList<PathData>();
    PlotInfo info=new PlotInfo();
    int count = 0;
    int plot_count = 0;

    public static void main(String[] args) {
        new PlotTestClient(args).run();
    }

    public PlotTestClient(String[] args) {
        NetworkTableInstance inst = NetworkTableInstance.getDefault();
        NetworkTable table = inst.getTable("datatable");
        String ip = "10.1.59.2";
        //if (args.length > 0)
        //    ip = args[0];
        System.out.println("new PlotTestClient "+ip);
        inst.startClient(ip);
        table.addEntryListener(this, kImmediate | kNew | kUpdate);
    }

    public void run() {
        while (true) {
            try {
                Thread.sleep(1000);
            } catch (InterruptedException ex) {
                System.out.println("exception)");
            }
        }
    }

    @Override
    public void valueChanged(NetworkTable table, String key, NetworkTableEntry entry, NetworkTableValue value,
            int flags) {
        if (key.equals("NewPlot")) {
            plot_count = (int) entry.getDouble(0.0);
            System.out.println("PlotTestClient NewPlot id=" + plot_count);
        }
        if (key.equals("PlotParams" + plot_count)) {
            list = new ArrayList<PathData>();
            double data[] = entry.getDoubleArray(new double[0]);
            info.id = (int) data[0];
            info.traces = (int) data[1];
            info.points = (int) data[2];
            info.mode = (int) data[3];
            //info.id = 0;
            count = 0;
            //System.out.println("PlotParams id:" + id + " traces:" + traces + "  points:" + points + " mode:" + mode);
            System.out.println(info);

        }
        if (key.equals("PlotData" + count)) {
            double data[] = entry.getDoubleArray(new double[0]);
            PathData pd = new PathData();
            //index = (int) data[0];
            pd.tm = data[1];
            for (int i = 0; i < data.length - 2; i++) {
                pd.d[i] = data[i + 2];
            }
            list.add(pd);
            count++;
        }
        if ((count >= info.points) && (info.points > 0) && (info.id == plot_count)) {
            System.out.println("PlotTestClient showing plot id:" + info.id);
            count = 0;
            plot_count++;
            // show each plot in a ew thread
            SwingUtilities.invokeLater(new Runnable() {
                public void run() {
                    //System.out.println("points " + info.points);
                    // System.out.println("Showing plot: Size = " + list.size());
                    JFrame frame = new PlotPath(list, info);
                    // frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                    frame.pack();
                    frame.setLocationRelativeTo(null);
                    frame.setVisible(true);
                }
            });
        }
    }
}
