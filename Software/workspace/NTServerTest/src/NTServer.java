import edu.wpi.first.wpilibj.networktables.NetworkTable;

public class NTServer {
	public static void main(String [] args) {
		new NTServer().run();
	}
	public void run(){
		NetworkTable table = NetworkTable.getTable("datatable");
		double x=0;
		double y=0;
		double d[]=new double[2];
		int cnt=0;
		while (true) {
			try {
				Thread.sleep(500);
			} catch (InterruptedException ex) {
				System.out.println("exception)");
			}
			d[0]=y;
			d[1]=x;
			table.putNumber("newdata", cnt);
			table.putNumberArray("vals", d);
			//table.putNumber("x", x);
			//table.putNumber("y", y);
			table.putNumber("enddata", cnt);
			x+=0.02;
			y+=0.5;
			cnt++;
		}
	}
}
