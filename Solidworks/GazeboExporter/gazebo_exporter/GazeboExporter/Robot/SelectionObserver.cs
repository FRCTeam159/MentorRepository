using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Robot
{
    public interface SelectionObserver
    {
        void SaveSelections();

        void CheckValidSelection(out bool isValid, int Id, int SelType, object selection);

        void UpdatePreviews(int Id, object selection);

        void SaveSpecificSelection(int Id);

        void NoSelections(int Id);
    }
}
