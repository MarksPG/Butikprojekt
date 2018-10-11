using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;


namespace Butik_PGCJ
{
    class MyForm : Form
    {
        public MyForm()
        {
            TableLayoutPanel outline = new TableLayoutPanel
            {
                RowCount = 4,
                ColumnCount = 3
            };
            Controls.Add(outline);
        }
    }
}
