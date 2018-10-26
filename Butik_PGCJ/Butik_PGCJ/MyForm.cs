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

    class Discount
    {
        public string DiscountName;
        public int DiscountValue;

        public static List<Discount> ReadDiscountFile()
        {
            string[] lines = File.ReadAllLines("Discounts.csv");
            List<Discount> discountItem = new List<Discount> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Discount d = new Discount
                {
                    DiscountName = values[0],
                    DiscountValue = int.Parse(values[1])
                };
                discountItem.Add(d);
            }
            return discountItem;
        }
    }

    //class Store
    //{
    //}

    class MyForm2 : Form
    {
        public MyForm2()
        {
            ClientSize = new Size(400, 300);

            TableLayoutPanel outlineReceipt = new TableLayoutPanel
            {
                RowCount = 4,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
            };
            Controls.Add(outlineReceipt);
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            outlineReceipt.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            ListView itemReceipt;
            itemReceipt = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false
            };
            outlineReceipt.SetRowSpan(itemReceipt, 1);
            outlineReceipt.SetColumnSpan(itemReceipt, 3);
            outlineReceipt.Controls.Add(itemReceipt, 1, 2);

            //ListViewItem item;

            Label receipt = new Label()
            {
                Text = "Kvitto",
                Font = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(receipt, 0, 0);

            Label quantity = new Label()
            {
                Text = "Antal",
                Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(quantity, 0, 2);

            Label itemName = new Label()
            {
                Text = "Artikel",
                Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(itemName, 1, 2);

            Label piecePrice = new Label()
            {
                Text = "á-Pris",
                Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(piecePrice, 2, 2);

            Dictionary<Guitar, int> shoppingCart = MyForm.shoppingCart;

        }
        
    }

    class MyForm : Form
    {
        Button doCheckout = new Button();
        Button addItemToCart = new Button();
        Button removeItemFromCart = new Button();
        Button addDiscount = new Button();
        Button saveCart = new Button();
        Button loadCart = new Button();
        Button clearCart = new Button();

        ColumnHeader cartColumnItem = new ColumnHeader();
        ColumnHeader cartColumnPrice = new ColumnHeader();

        double discountGlobalValue = 0;

        public static Dictionary<Guitar, int> shoppingCart = new Dictionary<Guitar, int>();

        Label itemListLabel = new Label();
        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();
        Label sumLabel = new Label();
        Label priceLabel = new Label();
        Label discountLabel = new Label();

        List<Guitar> shopItems = new List<Guitar>();

        ListBox itemList;

        ListView itemCart = new ListView();

        ListViewItem item;

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline = new TableLayoutPanel();
        TableLayoutPanel outlineBelowItemCart = new TableLayoutPanel();
        TableLayoutPanel outlinePriceInformation = new TableLayoutPanel();
        TableLayoutPanel outlineSaveAndLoad = new TableLayoutPanel();

        TextBox itemDescriptionTextbox = new TextBox();
        TextBox itemDescriptionAdditionalTextbox = new TextBox();
        TextBox discountTextbox = new TextBox();
        TextBox sumTextbox = new TextBox();
        TextBox priceTextbox = new TextBox();

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
            outline.SetRowSpan(itemList, 4);
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
                Dock = DockStyle.Fill,
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

            outlinePriceInformation = new TableLayoutPanel()
            {
                RowCount = 1,
                ColumnCount = 3,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(outlinePriceInformation, 1, 2);
            outline.SetColumnSpan(outlinePriceInformation, 2);

            //Addering av priceLabel
            priceLabel = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Text = "Pris:",
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
            };
            outlinePriceInformation.Controls.Add(priceLabel, 0, 0);

            //Addering av priceTextbox
            priceTextbox = new TextBox()
            {
                Anchor = AnchorStyles.Left,
                ReadOnly = true
            };
            outlinePriceInformation.Controls.Add(priceTextbox, 1, 0);

            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            //outlinePriceInformation.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

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
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                AutoSize = true
            };
            outline.Controls.Add(addDiscount, 2, 5);
            addDiscount.Click += discountButtonClicked;

            // Label som visar om rabattkoden är giltig.
            discountLabel = new Label()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Text = "",
                AutoSize = true
            };
            outline.Controls.Add(discountLabel, 1, 6);


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
            outline.SetRowSpan(itemCart, 4);
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
            outline.Controls.Add(outlineBelowItemCart, 3, 5);

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
                ReadOnly = true,
                Enabled = false
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
            doCheckout.Click += checkoutButtonClicked;

            //Addering av kolumner för outlineBelowItemCart
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

            //outlineBelowItemCart.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            // Lägger till panel outlineLoadAndSave
            outlineSaveAndLoad = new TableLayoutPanel()
            {
                RowCount = 1,
                ColumnCount = 2,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(outlineSaveAndLoad, 3, 6);

            // Knapp som sparar varukorg
            saveCart = new Button()
            {
                Text = "Spara varukorg",
                Anchor = AnchorStyles.None,
                AutoSize = true
            };
            outlineSaveAndLoad.Controls.Add(saveCart, 0, 0);
            saveCart.Click += saveAllItemsFromCart;

            loadCart = new Button()
            {
                Text = "Ladda varukorg",
                Anchor = AnchorStyles.None,
                AutoSize = true
            };
            outlineSaveAndLoad.Controls.Add(loadCart, 1, 0);
            loadCart.Click += loadSavedCart;

            outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            clearCart = new Button()
            {
                Text = "Töm varukorg",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(clearCart, 3, 7);
            clearCart.Click += removeAllItemsFromCart;

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

            //outline.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            shopItems = ReadVendorFile();
        }

        public List<Guitar> ReadVendorFile()
        {
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
                itemList.Items.Add(g.ItemName);
                shopItems.Add(g);
            }
            return shopItems;
        }

        private void saveAllItemsFromCart(object sender, EventArgs e)
        {
            string path = @"C:\Windows\Temp\savedCart.txt";
            File.WriteAllLines(path, shoppingCart.Select(kvp => string.Format("{0},{1},{2},{3},{4}", kvp.Key.ItemName, kvp.Key.ItemPrice, kvp.Key.ItemPic, kvp.Key.ItemDescr, kvp.Value)));
        }

        private void removeAllItemsFromCart(object sender, EventArgs e)
        {
            itemCart.Items.Clear();
            shoppingCart.Clear();
            sumTextbox.Text = "";
        }

        private void loadSavedCart(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(@"C:\Windows\Temp\savedCart.txt");
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
                shoppingCart.Add(g, int.Parse(values[4]));
            }
            UpdateListView(shoppingCart);
            UpdateSum();
            loadCart.Enabled = false;
        }

        private void discountButtonClicked(object sender, EventArgs e)
        {
            List<Discount> discountItem = Discount.ReadDiscountFile();
            string enteredCode = discountTextbox.Text;

            if (discountItem.Any(d => d.DiscountName == enteredCode))
            {
                int actualDiscount = discountItem.Where(d => d.DiscountName == enteredCode).Select(d => d.DiscountValue).Single();
                discountLabel.Text = "Grattis, koden är giltig!";
                discountGlobalValue = actualDiscount;
            }
            else
            {
                discountLabel.Text = "Tyvärr, koden är inte giltig!";
            }
            UpdateSum();
        }

        private void ItemListBoxClicked(object sender, EventArgs e)
        {
            Guitar g = shopItems[itemList.SelectedIndex];
            itemDescriptionTextbox.Text = g.ItemDescr;
            priceTextbox.Text = g.ItemPrice.ToString();
            itemPicture.Image = Image.FromFile(@"Pictures\" + g.ItemPic);
        }

        private void ItemCartAddClicked(object sender, EventArgs e)
        {
            Guitar g = shopItems[itemList.SelectedIndex];
            var index = shoppingCart.FirstOrDefault(x => x.Key.ItemName == g.ItemName);
            if (shoppingCart.ContainsKey(g) || index.Key != null)
            {
                shoppingCart[index.Key]++;
            }
            else
            {
                shoppingCart.Add(g, 1);
            }
            UpdateListView(shoppingCart);
            UpdateSum();
        }

        private void ItemCartRemClicked(object sender, EventArgs e)
        {
            string itemCheckTag = itemCart.SelectedItems[0].Tag.ToString();
            var itemToRemove = shoppingCart.Where(i => i.Key.ItemName == itemCheckTag).ToArray();
            foreach (var item in itemToRemove)
            {
                if (item.Value > 1)
                {
                    shoppingCart[item.Key] = item.Value - 1;
                    UpdateListView(shoppingCart);
                }
                else
                {
                    shoppingCart.Remove(item.Key);
                    itemCart.Items.Remove(itemCart.SelectedItems[0]);
                }
            }
            UpdateSum();
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

        private double UpdateSum()
        {
            sumTextbox.Text = String.Empty;
            double sumTotal = 0;

            if (discountGlobalValue > 0)
            {
                foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
                {
                    sumTotal += (pair.Key.ItemPrice * pair.Value) * (1 - discountGlobalValue / 100);
                }
            }
            else
            {
                foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
                {
                    sumTotal += pair.Key.ItemPrice * pair.Value;
                }
            }
            sumTextbox.Text = sumTotal.ToString();
            return sumTotal;
        }

        private void checkoutButtonClicked(object sender, EventArgs e)
        {
            var myForm = new MyForm2();
            myForm.StartPosition = FormStartPosition.CenterScreen;
            myForm.BackColor = Color.White;
            myForm.Show();


            //string test = UpdateSum().ToString();
            //MessageBox.Show(test);
        }
    }
}