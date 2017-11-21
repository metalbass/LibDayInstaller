using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibDayDataExtractor.Forms
{
    public partial class NearestNeighborPictureBox : PictureBox
    {
        public NearestNeighborPictureBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            eventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            base.OnPaint(eventArgs);
        }
    }
}
