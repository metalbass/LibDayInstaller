using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibDayDataExtractor.Forms
{
    public partial class ExceptionForm : Form
    {
        public ExceptionForm(Exception e) : this(e.Message, e.StackTrace)
        {
        }

        public ExceptionForm(string message, string stackTrace)
        {
            InitializeComponent();

            m_messageTextBox.Text = message;
            m_stackTraceTextBox.Text = stackTrace;
        }

        public static void Show(Exception e)
        {
            using (var form = new ExceptionForm(e))
            {
                form.ShowDialog();
            }
        }

        public static void Show(string message, string stackTrace)
        {
            using (var form = new ExceptionForm(message, stackTrace))
            {
                form.ShowDialog();
            }
        }
    }
}
