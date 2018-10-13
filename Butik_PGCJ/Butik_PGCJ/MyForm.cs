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
    class Guitar
    {
        public string ItemName;
        public int ItemPrice;
        public string ItemPic;
        public string ItemDescr;
    }

    class MyForm : Form
    {
        Button buttonCheckout = new Button();
        Button addItemToCart = new Button();
        Button removeItemFromCart = new Button();

        ColumnHeader cartColumnItem = new ColumnHeader();
        ColumnHeader cartColumnPrice = new ColumnHeader();

        Label itemListLabel = new Label();
        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();

        ListBox itemList;

        ListView itemCart = new ListView();

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline = new TableLayoutPanel();

        TextBox itemDescriptionTextbox = new TextBox();
        TextBox itemDescriptionAdditionalTextbox = new TextBox();

        List<Guitar> shopItems;

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
            };
            Controls.Add(outline);

            //---------------Markerar början för kolumn 1---------------

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
                Text = "Beskrivning av vara",
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
            };
            outline.Controls.Add(itemList);
            outline.SetRowSpan(itemList, 5);
            itemList.SelectedIndexChanged += ItemListBoxClicked;

            //---------------Markerar slutet för kolumn 1---------------


            //---------------Markerar början för kolumn 2 och 3---------------

            //Picturebox
            itemPicture = new PictureBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            outline.Controls.Add(itemPicture, 1, 1);

            //Textbox med huvudinformation om varan
            itemDescriptionTextbox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
            };
            outline.Controls.Add(itemDescriptionTextbox, 2, 1);

            //Ytterligare textbox fast med kanske någon annan typ av information??
            itemDescriptionAdditionalTextbox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Text = "Här kan man skriva något mer om man vill",
            };
            outline.Controls.Add(itemDescriptionAdditionalTextbox, 1, 2);
            outline.SetColumnSpan(itemDescriptionAdditionalTextbox, 2);

            //Knapp för att lägga till vara i itemCart, från itemList.
            addItemToCart = new Button()
            {
                Anchor = AnchorStyles.None,
                Text = "Lägg till vara -->",
                AutoSize = true
            };
            outline.Controls.Add(addItemToCart, 1, 3);
            outline.SetColumnSpan(addItemToCart, 2);
            addItemToCart.Click += ItemCartAddClicked;

            //Knapp för att ta bort vara från itemCart
            removeItemFromCart = new Button()
            {
                Anchor = AnchorStyles.None,
                Text = "<-- Ta bort vara",
                AutoSize = true
            };
            outline.Controls.Add(removeItemFromCart, 1, 4);
            outline.SetColumnSpan(removeItemFromCart, 2);

            //---------------Markerar slutet för kolumn 2 och 3---------------


            //---------------Markerar början för kolumn 4---------------

            //ListView där adderade varor visas. Här är tanken att
            //man ska kunna radera varor också. Två kolumner, varan och priset.
            itemCart = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details
            };
            outline.SetRowSpan(itemCart, 5);
            outline.Controls.Add(itemCart, 3, 1);

            //Kolumn där varans namn bör synas
            cartColumnItem = new ColumnHeader()
            {
                Text = "Vara",
                Width = 150
            };

            //Kolumn där varans pris bör synas
            /*cartColumnPrice = new ColumnHeader()
            {
                Text = "Pris",
                Width = 80
            };*/

            //Lägger till kolumnHeader för "Vara".
            itemCart.Columns.Add(cartColumnItem);

            //Knapp som "checka ut"/"köper".
            buttonCheckout = new Button()
            {
                Anchor = AnchorStyles.Top,
                Text = "Checkout"
            };
            outline.Controls.Add(buttonCheckout, 3, 6);

            //Addering av rader.
            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            for (int i = 0; i < 6; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }
            
            //Addering av kolumner.
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            outline.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            //Inläsning från csv-fil "VendingSupply.csv"
            string[] lines = File.ReadAllLines("VendingSupply.csv");
            shopItems = new List<Guitar> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Guitar g = new Guitar
                {
                    ItemName = values[0],
                    ItemPrice = int.Parse(values[1]),
                    ItemPic = values[2],
                    ItemDescr = values[3]
                };
                itemList.Items.Add(g.ItemName + " Pris :" + g.ItemPrice);
                shopItems.Add(g);
            }
        }
        private void ItemListBoxClicked(object sender, EventArgs e)
        {
            Guitar g = shopItems[itemList.SelectedIndex];
            itemDescriptionTextbox.Text = g.ItemDescr;
            itemPicture.Image = Image.FromFile(@"Pictures\" + g.ItemPic);
        }

        private void ItemCartAddClicked(object sender, EventArgs e)
        {
            Guitar g = shopItems[itemList.SelectedIndex];
            itemCart.Items.Add(g.ItemName);
        }
    }
}

