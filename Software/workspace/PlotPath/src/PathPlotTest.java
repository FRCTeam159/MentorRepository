import java.util.ArrayList;
import java.util.Random;

import javax.swing.JFrame;
import javax.swing.SwingUtilities;

public class PathPlotTest {
	private static void createAndShowGui() {
		ArrayList<PathData> data = new ArrayList<>();
		Random random = new Random();
		int maxDataPoints = 200;
		int maxScore = 10;
		double tm=0;

		for (int i = 0; i < maxDataPoints; i++) {
			PathData pd=new PathData();
			pd.tm=tm;
			pd.d[0]=(double) random.nextDouble() * maxScore;
			data.add(pd);
			tm+=0.02;
		}
		JFrame frame = new PlotPath(data,1);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.pack();
		frame.setLocationRelativeTo(null);
		frame.setVisible(true);
	}

	public static void main(String[] args) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				createAndShowGui();
			}
		});
	}
}
