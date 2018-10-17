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
        Button doCheckout = new Button();
        Button addItemToCart = new Button();
        Button removeItemFromCart = new Button();
        Button addDiscount = new Button();

        ColumnHeader cartColumnItem = new ColumnHeader();
        ColumnHeader cartColumnPrice = new ColumnHeader();

        Dictionary<Guitar, int> shoppingCart = new Dictionary<Guitar, int>();

        Label itemListLabel = new Label();
        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();
        Label sumLabel = new Label();

        List<Guitar> shopItems = new List<Guitar>();

        ListBox itemList;

        ListView itemCart = new ListView();

        ListViewItem item;

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline = new TableLayoutPanel();
        TableLayoutPanel outlineBelowItemCart = new TableLayoutPanel();

        TextBox itemDescriptionTextbox = new TextBox();
        TextBox itemDescriptionAdditionalTextbox = new TextBox();
        TextBox discountTextbox = new TextBox();
        TextBox sumTextbox = new TextBox();

        public MyForm()
        {
            //Talar om storleken på winform vid uppstart
            ClientSize = new Size(850, 550);

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
            outline.Controls.Add(itemListLabel, 0, 0);

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

            //Label för beskrivning av vara
            itemDescriptionLabel = new Label()
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Beskrivning av vara",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };
            outline.Controls.Add(itemDescriptionLabel, 1, 0);
            outline.SetColumnSpan(itemDescriptionLabel, 2);

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
            removeItemFromCart.Click += ItemCartRemClicked;

            //Textbox där användaren kan skriva in en rabattkod
            discountTextbox = new TextBox()
            {
                Text = "Skriv in rabattkod här...",
                Dock = DockStyle.Bottom,
            };
            outline.Controls.Add(discountTextbox, 1, 5);

            //Knapp där användaren trycker för att applicera rabattkoden
            addDiscount = new Button()
            {
                Text = "Applicera rabatt",
                Anchor = (AnchorStyles.Bottom | AnchorStyles.Left),
                AutoSize = true
            };
            outline.Controls.Add(addDiscount, 2, 5);


            //---------------Markerar slutet för kolumn 2 och 3---------------


            //---------------Markerar början för kolumn 4---------------

            //Label för varukorg
            itemCartLabel = new Label()
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Varukorg",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
            };
            outline.Controls.Add(itemCartLabel, 3, 0);

            //ListView där adderade varor visas.
            itemCart = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false
            };
            outline.SetRowSpan(itemCart, 5);
            outline.Controls.Add(itemCart, 3, 1);

            itemCart.Columns.Add("Vara");
            itemCart.Columns.Add("Antal");
            itemCart.Columns.Add("Pris");

            //Lägger till en ny Panel l
            outlineBelowItemCart = new TableLayoutPanel()
            {
                RowCount = 1,
                ColumnCount = 3,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(outlineBelowItemCart, 3, 6);

            //Addering av sumLabel
            sumLabel = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Text = "Summa:",
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
            };
            outlineBelowItemCart.Controls.Add(sumLabel, 0, 0);

            //Addering av sumTextbox
            sumTextbox = new TextBox()
            {
                Anchor = AnchorStyles.Left,
            };
            outlineBelowItemCart.Controls.Add(sumTextbox, 1, 0);

            //Knapp som "checka ut"/"köper".
            doCheckout = new Button()
            {
                Text = "Checkout",
                Anchor = AnchorStyles.None,
                AutoSize = true
            };
            outlineBelowItemCart.Controls.Add(doCheckout, 2, 0);

            //Addering av kolumner för outlineBelowItemCart
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

            outlineBelowItemCart.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            //---------------Markerar slutet för kolumn 4---------------

            //Addering av rader för outline.
            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            for (int i = 0; i < 6; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }

            //Addering av kolumner för outline.
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            outline.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            //Inläsning från csv-fil "VendingSupply.csv" och addering till Lista fylld
            //med objekt av typen klassen Guitar
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
            if (shoppingCart.ContainsKey(g) == false)
            {
                shoppingCart.Add(g, 1);
                //item = new ListViewItem(g.ItemName);
                //itemCart.Items.Add(item);
            }
            else
            {
                shoppingCart[g]++;
                //item.SubItems.Add(shoppingCart[g].ToString());
            }
            UpdateListView(shoppingCart);
        }

        private void ItemCartRemClicked(object sender, EventArgs e)
        {
            string itemCheckTag = itemCart.SelectedItems[0].Tag.ToString();
            var itemToRemove = shoppingCart.Where(i => i.Key.ItemName == itemCheckTag).ToArray();
            foreach (var item in itemToRemove)
            {
                if (item.Value > 1)
                {
                    shoppingCart[item.Key] = item.Value-1;
                    UpdateListView(shoppingCart);
                }
                else
                {
                    shoppingCart.Remove(item.Key);
                    itemCart.Items.Remove(itemCart.SelectedItems[0]);
                }
            }
        }

        private void UpdateListView(Dictionary<Guitar, int> shoppingCart)
        {
            itemCart.Items.Clear();

            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                item = new ListViewItem(pair.Key.ItemName);
                item.SubItems.Add(pair.Value.ToString());
                item.SubItems.Add(pair.Key.ItemPrice.ToString());
                item.Tag = pair.Key.ItemName;
                itemCart.Items.Add(item);
            }
        }
    }
}