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
    class Product
    {
        public string Type { get; set; }
        public string ItemName { get; set; }
        private int itemPrice;
        public string ItemPic { get; set; }
        public string ItemDescr { get; set; }
        public int KeyValue { get; set; }
        public static List<Product> ShopItems { get; set; }

        public int ItemPrice
        {
            get
            {
                return itemPrice;
            }
            set
            {
                itemPrice = value;
            }
        }

        public static List<Product> ReadVendorFile()
        {
            string[] lines = File.ReadAllLines("Products.csv");
            List<Product> shopItems = new List<Product> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Product p = new Product
                {
                    Type = values[0],
                    ItemName = values[1],
                    itemPrice = int.Parse(values[2]),
                    ItemPic = values[3],
                    ItemDescr = values[4]
                };
                MyForm.listBoxItems.Items.Add(p.ItemName);
                shopItems.Add(p);
            }
            return shopItems;
        }

        public static void PopulateListBox(List<Product> tempShopList)
        {
            MyForm.listBoxItems.Items.Clear();
            foreach (Product p in tempShopList)
            {
                MyForm.listBoxItems.Items.Add(p.ItemName);
            }
        }
    }

    class ShoppingCart
    {
        private Dictionary<Product, int> shoppingBasket = new Dictionary<Product, int>();

        public Dictionary<Product, int> ShoppingBasket
        {
            get
            {
                return shoppingBasket;
            }
            set
            {
                if (shoppingBasket.Min(x => x.Value) < 0)
                {
                    MessageBox.Show("Antalet kan inte vara negativt");
                }
                else
                {
                    shoppingBasket = value;
                }
            }
        }

        public void SaveCart()
        {
            string path = @"C:\Windows\Temp\savedCart.txt";
            File.WriteAllLines(path, shoppingBasket.Select(kvp => string.Format("{0},{1},{2},{3},{4},{5}", kvp.Key.Type, kvp.Key.ItemName, kvp.Key.ItemPrice, kvp.Key.ItemPic, kvp.Key.ItemDescr, kvp.Value)));
        }

        public void LoadCart()
        {
            string[] lines = File.ReadAllLines(@"C:\Windows\Temp\savedCart.txt");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Product p = new Product
                {
                    Type = values[0],
                    ItemName = values[1],
                    ItemPrice = int.Parse(values[2]),
                    ItemPic = values[3],
                    ItemDescr = values[4],
                    KeyValue = int.Parse(values[5])
                };

                var index = shoppingBasket.FirstOrDefault(x => x.Key.ItemName == p.ItemName);
                if (shoppingBasket.ContainsKey(p) || index.Key != null)
                {
                    shoppingBasket[index.Key] += p.KeyValue;
                }
                else
                {
                    shoppingBasket.Add(p, p.KeyValue);
                }
            }
        }

        public void AddToBasket(Product p)
        {
            var index = shoppingBasket.FirstOrDefault(x => x.Key.ItemName == p.ItemName);
            if (shoppingBasket.ContainsKey(p) || index.Key != null)
            {
                shoppingBasket[index.Key]++;
            }
            else
            {
                shoppingBasket.Add(p, 1);
            }
        }

        public void RemoveFromBasket(ListView itemCart)
        {
            string itemCheckTag = itemCart.SelectedItems[0].Tag.ToString();
            var itemToRemove = shoppingBasket.Where(i => i.Key.ItemName == itemCheckTag).ToArray();
            foreach (var i in itemToRemove)
            {
                if (i.Value > 1)
                {
                    shoppingBasket[i.Key] = i.Value - 1;
                    UpdateListView(itemCart);
                }
                else
                {
                    shoppingBasket.Remove(i.Key);
                    itemCart.Items.Remove(itemCart.SelectedItems[0]);
                }
            }
        }

        public void UpdateListView(ListView itemCart)
        {
            itemCart.Items.Clear();

            foreach (KeyValuePair<Product, int> pair in shoppingBasket)
            {
                ListViewItem item = new ListViewItem(pair.Key.ItemName);
                item.SubItems.Add(pair.Value.ToString());
                item.SubItems.Add(pair.Key.ItemPrice.ToString());
                item.Tag = pair.Key.ItemName;
                itemCart.Items.Add(item);
            }
        }

        public double CalculateDictionary()
        {
            double sumTotal = 0;
            foreach (KeyValuePair<Product, int> pair in shoppingBasket)
            {
                sumTotal += pair.Key.ItemPrice * pair.Value;
            }
            return sumTotal;
        }
    }

    class Discount
    {
        public string DiscountName { get; set; }
        public int DiscountValue { get; set; }
        private static double actualDiscountValue;

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
                if (int.Parse(values[1]) > 50)
                {
                    throw new InvalidOperationException("Rabattsatsen överstiger högsta tillåtna. Kontrollera CSV-filen.");
                }
                else
                {
                    discountItem.Add(d);
                }
            }
            return discountItem;
        }

        public static double CalculateDiscountDictionary(Dictionary<Product, int> shoppingBasket)
        {
            //double discountValue = actualDiscountValue;
            double sumTotal = 0;
            foreach (KeyValuePair<Product, int> pair in shoppingBasket)
            {
                sumTotal += (pair.Key.ItemPrice * pair.Value) * (1 - actualDiscountValue / 100);
            }
            return sumTotal;
        }

        public void CheckDiscountCode(TextBox discountTextbox, Label discountLabel, Button addDiscount)
        {
            List<Discount> discountItem = Discount.ReadDiscountFile();
            string enteredCode = discountTextbox.Text;

            if (discountItem.Any(d => d.DiscountName == enteredCode))
            {
                double findDiscountValue = discountItem.Where(d => d.DiscountName == enteredCode).Select(d => d.DiscountValue).Single();
                discountLabel.Text = "Grattis, koden är giltig och ger dig " + findDiscountValue + "% på allt du köper!";
                addDiscount.Enabled = false;
                discountTextbox.Text = "Du har angivit en rabattkod!";
                discountTextbox.Enabled = false;

                actualDiscountValue = findDiscountValue;
            }
            else
            {
                discountLabel.Text = "Tyvärr, koden är inte giltig!";
            }
        }
    }

    class MyForm : Form
    {
        // Global initializing
        public static ListBox listBoxItems;

        // Static initializing
        static Label sumLabel;
        public static ShoppingCart s = new ShoppingCart();
        public static Discount d = new Discount();


        // Non global initializing
        Button doCheckout;
        Button addItemToCart;
        Button removeItemFromCart;
        Button addDiscount;
        Button saveCart;
        Button loadCart;
        Button clearCart;
        Button loadGuitars;
        Button loadAccessories;

        Label itemDescriptionLabel;
        Label itemCartLabel;
        Label actualPriceLabel;
        Label discountLabel;

        List<Product> shopItemsList = new List<Product>();
        List<Product> tempShopList = new List<Product>();

        ListView itemCart;

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline;
        TableLayoutPanel outlineBelowItemCart;
        TableLayoutPanel outlineBelowShopItems;
        TableLayoutPanel outlinePriceInformation;
        TableLayoutPanel outlineSaveAndLoad;
        TableLayoutPanel outlineBackgroundImage;
        TableLayoutPanel outlineDiscountArea;

        TextBox itemDescriptionTextbox;
        TextBox discountTextbox;

        public MyForm()
        {
            // Sets clientsize
            ClientSize = new Size(840, 570);

            // Icon for store
            Icon = new Icon(@"Pictures\guitar_icon.ico");

            // Name of store
            Text = "PGCJ Gitarraffär - plocka dina strängar online";

            // Background color of MyForm
            BackColor = SystemColors.InactiveBorder;

            // Outlines
            outline = CreateOutline(8, 4);
            Controls.Add(outline);

            outlineBelowItemCart = CreateOutline(1, 3);
            outline.Controls.Add(outlineBelowItemCart, 3, 5);

            outlineBelowShopItems = CreateOutline(1, 2);
            outline.Controls.Add(outlineBelowShopItems, 0, 5);

            outlineBackgroundImage = CreateOutline(1, 1);
            outline.Controls.Add(outlineBackgroundImage, 0, 6);
            outlineBackgroundImage.SetColumnSpan(outlineBackgroundImage, 3);
            outlineBackgroundImage.SetRowSpan(outlineBackgroundImage, 2);
            outlineBackgroundImage.BackgroundImage = new Bitmap(@"Pictures\bkgr-opacity.png");
            outlineBackgroundImage.BackgroundImageLayout = ImageLayout.None;

            outlinePriceInformation = CreateOutline(1, 3);
            outline.Controls.Add(outlinePriceInformation, 1, 2);
            outline.SetColumnSpan(outlinePriceInformation, 2);

            outlineDiscountArea = CreateOutline(2, 2);
            outline.Controls.Add(outlineDiscountArea, 1, 5);
            outlineDiscountArea.SetColumnSpan(outlineDiscountArea, 2);

            outlineSaveAndLoad = CreateOutline(1, 2);
            outline.Controls.Add(outlineSaveAndLoad, 3, 6);

            // Rows & Columns for outlines
            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            for (int i = 0; i < 5; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }

            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            for (int i = 0; i < 2; i++) { outlineBelowShopItems.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); }

            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 67));

            for (int i = 0; i < 2; i++) { outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); }

            // Labels
            outline.Controls.Add(CreateLabel(AnchorStyles.None, true, "Utbud"), 0, 0); // itemListLabel

            itemDescriptionLabel = CreateLabel(AnchorStyles.None, true, "Beskrivning av vara");
            outline.Controls.Add(itemDescriptionLabel, 1, 0);
            outline.SetColumnSpan(itemDescriptionLabel, 2);

            outlinePriceInformation.Controls.Add(CreateLabel(AnchorStyles.Right, true, "Pris:"), 0, 0); // priceLabel

            actualPriceLabel = CreateLabel(AnchorStyles.Left, true, "");
            outlinePriceInformation.Controls.Add(actualPriceLabel, 1, 0);

            discountLabel = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true,
                Text = "",
                Font = new Font("Arial", 8)
            };
            outlineDiscountArea.Controls.Add(discountLabel, 0, 1);
            outline.SetColumnSpan(discountLabel, 2);

            outlineBelowItemCart.Controls.Add(CreateLabel(AnchorStyles.Left, true, "Summa:"), 0, 0); // sumLabel

            itemCartLabel = CreateLabel(AnchorStyles.None, true, "Varukorg");
            outline.Controls.Add(itemCartLabel, 3, 0);

            sumLabel = new Label
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Arial", 11)
            };
            outlineBelowItemCart.Controls.Add(sumLabel, 1, 0);

            // Buttons
            addItemToCart = new Button
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Lägg till vara -->",
                Size = new Size(150, 40),
                Font = new Font("Arial", 9),
                BackColor = Color.DarkKhaki
            };
            outline.Controls.Add(addItemToCart, 1, 3);
            outline.SetColumnSpan(addItemToCart, 2);
            addItemToCart.Click += ItemCartAddClicked;

            removeItemFromCart = new Button
            {
                Anchor = AnchorStyles.Top,
                AutoSize = true,
                Text = "<-- Ta bort vara",
                Size = new Size(150, 40),
                Font = new Font("Arial", 9),
                BackColor = Color.DarkKhaki
            };
            outline.Controls.Add(removeItemFromCart, 1, 4);
            outline.SetColumnSpan(removeItemFromCart, 2);
            removeItemFromCart.Click += ItemCartRemClicked;

            loadGuitars = CreateButton(AnchorStyles.None, "Visa gitarrer");
            outlineBelowShopItems.Controls.Add(loadGuitars, 0, 0);
            loadGuitars.Click += LoadGuitarToItemListView;

            loadAccessories = CreateButton(AnchorStyles.None, "Visa tillbehör");
            outlineBelowShopItems.Controls.Add(loadAccessories, 1, 0);
            loadAccessories.Click += LoadAccessoryToItemListView;

            saveCart = CreateButton(AnchorStyles.None, "Spara varukorg");
            outlineSaveAndLoad.Controls.Add(saveCart, 0, 0);
            saveCart.Click += SaveAllItemsFromCart;

            loadCart = CreateButton(AnchorStyles.None, "Ladda varukorg");
            outlineSaveAndLoad.Controls.Add(loadCart, 1, 0);
            loadCart.Click += LoadSavedCart;

            doCheckout = CreateButton(AnchorStyles.None, "Checkout");
            outlineBelowItemCart.Controls.Add(doCheckout, 2, 0);
            doCheckout.Click += CheckoutButtonClicked;

            addDiscount = CreateButton(AnchorStyles.None, "Applicera rabatt");
            outlineDiscountArea.Controls.Add(addDiscount, 1, 0);
            addDiscount.Click += DiscountButtonClicked;

            clearCart = new Button()
            {
                Text = "Töm varukorg",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(clearCart, 3, 7);
            clearCart.Click += RemoveAllItemsFromCart;

            // Textbox
            itemDescriptionTextbox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Arial", 9)
            };
            outline.Controls.Add(itemDescriptionTextbox, 2, 1);

            discountTextbox = new TextBox()
            {
                Text = "Skriv in rabattkod här...",
                Anchor = AnchorStyles.None,
                Size = new Size(140, 0)
            };
            outlineDiscountArea.Controls.Add(discountTextbox, 0, 0);
            discountTextbox.Click += DiscountTextBoxClicked;
            discountTextbox.KeyDown += DiscountTextBox_KeyDown;

            // ListBoxItems (products)
            listBoxItems = new ListBox()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10)
            };
            outline.Controls.Add(listBoxItems);
            outline.SetRowSpan(listBoxItems, 4);
            listBoxItems.SelectedIndexChanged += ItemListBoxClicked;

            // ListView (cart)
            itemCart = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false,
                Font = new Font("Arial", 10)
            };
            outline.SetRowSpan(itemCart, 4);
            outline.Controls.Add(itemCart, 3, 1);

            itemCart.Columns.Add("Vara").Width = 125;
            itemCart.Columns.Add("Antal").Width = 45;
            itemCart.Columns.Add("Pris").Width = 50;

            // Picturebox
            itemPicture = new PictureBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            outline.Controls.Add(itemPicture, 1, 1);

            shopItemsList = Product.ReadVendorFile();
            tempShopList = shopItemsList.ToList();
        }

        private void LoadGuitarToItemListView(object sender, EventArgs e)
        {
            tempShopList.Clear();
            foreach (Product p in shopItemsList)
            {
                if (p.Type == "gitarrer")
                {
                    tempShopList.Add(p);
                }
            }
            Product.PopulateListBox(tempShopList);
        }

        private void LoadAccessoryToItemListView(object sender, EventArgs e)
        {
            tempShopList.Clear();
            foreach (Product p in shopItemsList)
            {
                if (p.Type == "tillbehör")
                {
                    tempShopList.Add(p);
                }
            }
            Product.PopulateListBox(tempShopList);
        }

        private void SaveAllItemsFromCart(object sender, EventArgs e)
        {
            s.SaveCart();
        }

        public void LoadSavedCart(object sender, EventArgs e)
        {
            s.LoadCart();
            s.UpdateListView(itemCart);
            UpdateSum(Discount.CalculateDiscountDictionary(s.ShoppingBasket));
            loadCart.Enabled = false;
        }

        private void RemoveAllItemsFromCart(object sender, EventArgs e)
        {
            itemCart.Items.Clear();
            s.ShoppingBasket.Clear();
            sumLabel.Text = "";
        }

        private void DiscountTextBoxClicked(object sender, EventArgs e)
        {
            discountTextbox.Text = string.Empty;
        }

        private void DiscountTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DiscountButtonClicked(this, new EventArgs());
            }
        }

        private void DiscountButtonClicked(object sender, EventArgs e)
        {
            d.CheckDiscountCode(discountTextbox, discountLabel, addDiscount);
            UpdateSum(Discount.CalculateDiscountDictionary(s.ShoppingBasket));
        }

        private void ItemListBoxClicked(object sender, EventArgs e)
        {
            if (listBoxItems.SelectedIndex < 0)
            {
                return;
            }
            else
            {
                Product p = tempShopList[listBoxItems.SelectedIndex];
                itemDescriptionTextbox.Text = p.ItemDescr;
                actualPriceLabel.Text = p.ItemPrice.ToString() + " kr";
                itemPicture.Image = Image.FromFile(@"Pictures\" + p.ItemPic);
            }
        }

        private void ItemCartAddClicked(object sender, EventArgs e)
        {
            try
            {
                Product p = tempShopList[listBoxItems.SelectedIndex];
                s.AddToBasket(p);
                s.UpdateListView(itemCart);
                UpdateSum(Discount.CalculateDiscountDictionary(s.ShoppingBasket));
            }
            catch
            {
                MessageBox.Show("Välj en vara att lägga till");
            }
        }

        private void ItemCartRemClicked(object sender, EventArgs e)
        {
            try
            {
                s.RemoveFromBasket(itemCart);
                UpdateSum(Discount.CalculateDiscountDictionary(s.ShoppingBasket));
            }
            catch
            {
                MessageBox.Show("Välj en vara att ta bort");
            }
        }

        private void UpdateSum(double sum)
        {
            sumLabel.Text = sum.ToString() + " kr";
        }

        private void CheckoutButtonClicked(object sender, EventArgs e)
        {
            var myForm = new MyForm2();
            myForm.StartPosition = FormStartPosition.CenterScreen;
            myForm.BackColor = Color.White;
            myForm.Show();
        }

        private static TableLayoutPanel CreateOutline(int row, int column)
        {
            return new TableLayoutPanel
            {
                RowCount = row,
                ColumnCount = column,
                Dock = DockStyle.Fill
            };
        }

        private static Label CreateLabel(AnchorStyles anchor, bool value, string name)
        {
            return new Label
            {
                Anchor = anchor,
                AutoSize = value,
                Text = name,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
        }

        private static Button CreateButton(AnchorStyles anchor, string name)
        {
            return new Button
            {
                Anchor = anchor,
                AutoSize = true,
                Text = name,
            };
        }
    }

    class MyForm2 : Form
    {
        public MyForm2()
        {
            // Icon for receipt
            Icon = new Icon(@"Pictures\guitar_icon.ico");

            // Name of receipt
            Text = "PGCJ Gitarraffär - Tack för att du handlat hos oss.";

            // Sets receipt-size
            ClientSize = new Size(400, 300);

            // Outline
            TableLayoutPanel outlineReceipt = new TableLayoutPanel
            {
                RowCount = 6,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            Controls.Add(outlineReceipt);

            // Rows & Columns
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 2; i++) { outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); }

            // Calculate discount on Dictionary
            double sumTotal = Discount.CalculateDiscountDictionary(MyForm.s.ShoppingBasket);
            double sumDifference = MyForm.s.CalculateDictionary() - sumTotal;

            // Labels
            Label receipt = new Label()
            {
                Anchor = AnchorStyles.Left,
                Text = "Kvitto",
                Font = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(receipt, 0, 0);

            Label totalPriceLabel = new Label()
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Text = sumTotal + " kr",
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            outlineReceipt.Controls.Add(totalPriceLabel, 2, 5);

            Label sumDifferenceLabelText = new Label()
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Text = "Rabattkoden du angav har sparat dig:",
                Font = new Font("Arial", 10),
                ForeColor = Color.Red,
                AutoSize = true
            };
            if (sumDifference > 0)
            {
                outlineReceipt.Controls.Add(sumDifferenceLabelText, 0, 4);
                outlineReceipt.SetColumnSpan(sumDifferenceLabelText, 2);
            }

            Label sumDifferencePriceLabel = new Label()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Text = sumDifference + "kr",
                Font = new Font("Arial", 10),
                ForeColor = Color.Red,
                AutoSize = true
            };
            if (sumDifference > 0)
            {
                outlineReceipt.Controls.Add(sumDifferencePriceLabel, 2, 4);
            }

            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "Artikel"), 0, 2); // itemName
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "Antal"), 1, 2); // quantity
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Right | AnchorStyles.Top, "Totalpris:"), 1, 5); // totalPriceLabelText
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "á-Pris"), 2, 2); // pricePrice

            // DataGridView
            DataGridView dtgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                BackgroundColor = Color.White,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
            };
            outlineReceipt.Controls.Add(dtgv, 0, 3);
            outlineReceipt.SetColumnSpan(dtgv, 3);

            // DataGridView cellstyle
            dtgv.DefaultCellStyle.SelectionBackColor = dtgv.DefaultCellStyle.BackColor;
            dtgv.DefaultCellStyle.SelectionForeColor = dtgv.DefaultCellStyle.ForeColor;

            // DataGridViewColumns
            DataGridViewTextBoxColumn itemNameGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 51,
                ReadOnly = true,
            };
            DataGridViewTextBoxColumn quantityGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 25,
                ReadOnly = true,
            };
            DataGridViewTextBoxColumn priceGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 23,
                ReadOnly = true,
            };

            // Adding columns to DataGridView
            dtgv.Columns.AddRange(new DataGridViewColumn[]
            {
                itemNameGrid, quantityGrid, priceGrid
            });

            // Sums dictionary and displays at receipt
            foreach (KeyValuePair<Product, int> pair in MyForm.s.ShoppingBasket)
            {
                dtgv.Rows.Add(pair.Key.ItemName, pair.Value, pair.Key.ItemPrice + " kr");
            }
        }

        private static Label CreateLabel(AnchorStyles anchor, string name)
        {
            return new Label
            {
                Anchor = anchor,
                Text = name,
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
        }
    }
}