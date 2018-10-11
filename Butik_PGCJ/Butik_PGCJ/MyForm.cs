using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;


namespace Butik_PGCJ
{
    class MyForm : Form
    {
        TableLayoutPanel outline = new TableLayoutPanel();

        Label itemListLabel = new Label();
        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();

        ListBox itemList = new ListBox();

        public MyForm()
        {
            //Talar om storleken på winform vid uppstart
            ClientSize = new Size(800, 550);

            //Skapar rutnät
            outline = new TableLayoutPanel
            {
                RowCount = 8,
                ColumnCount = 4,
                Dock = DockStyle.Fill,
                Size = new System.Drawing.Size(900, 500)
            };
            Controls.Add(outline);

            //Label för utbud av varor
            itemListLabel = new Label()
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Utbud",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
            };
            outline.Controls.Add(itemListLabel);

            //Label för beskrivning av vara
            itemDescriptionLabel = new Label()
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Beskrivning",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };
            outline.Controls.Add(itemDescriptionLabel);
            outline.SetColumnSpan(itemDescriptionLabel, 2);

            //Label för varukorg
            itemCartLabel = new Label()
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Varukorg",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
            };
            outline.Controls.Add(itemCartLabel);

            //Lista över tillgänliga varor i shoppen
            itemList = new ListBox()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 10),
            };
            outline.Controls.Add(itemList);
            outline.SetRowSpan(itemList, 5);
            
            //Adderar några testobjekt. Endast för att se något i itemList.
            itemList.Items.AddRange(new object[]
            {
                "Martin",
                "Fender",
                "Taylor",
                "Gibson",
                "Ibanze",
            });

            //Addering av rader och kolumner, samt storlektsmall.
            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            for (int i = 0; i < 8; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }           
            for (int i = 0; i < 4; i++) { outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); }

            outline.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
        }
    }
}
