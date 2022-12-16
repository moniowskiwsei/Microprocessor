using Microsoft.Win32;
using System.Drawing.Text;
using System.Globalization;

namespace microprocessor
{
    public partial class Form1 : Form
    {
        private TextBox command_input;
        private Label error_label;
        private Button execute_button;
        private Button next_button;
        private Button prev_button;
        private Label page_label;
        private List<Label> memory_label = new List<Label>();
        private IDictionary <string, Label> registers_label = new Dictionary<string, Label>();

        private int memoryPage = 0;
        private int memoryWidth = 16;
        private Microprocessor microprocessor = new Microprocessor();
        private int maxPage;
        public Form1()
        {
            InitializeComponent();

            InitUI();
        }

        private void InitUI()
        {
            this.maxPage = Microprocessor.memorySize / (this.memoryWidth * this.memoryWidth) - 1;
            initCommandElements();
            initMemoryElements();
            initRegistersElements();
            updateUI();
        }

        private void initCommandElements()
        {
            this.command_input = new System.Windows.Forms.TextBox();
            this.error_label = new System.Windows.Forms.Label();
            this.execute_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // command_input
            // 
            this.command_input.Location = new System.Drawing.Point(10, 45);
            this.command_input.Name = "command_input";
            this.command_input.Size = new System.Drawing.Size(250, 50);
            this.command_input.Font = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.command_input.TabIndex = 0;

            makeTitle("Enter your command", 10, 10);
            // 
            // error_label
            // 
            this.error_label.AutoSize = true;
            this.error_label.Location = new System.Drawing.Point(10, 80);
            this.error_label.Name = "error_label";
            this.error_label.Size = new System.Drawing.Size(119, 15);
            this.error_label.ForeColor = System.Drawing.Color.Red;
            this.error_label.Text = "";
            // 
            // execute_button
            // 
            this.execute_button.Location = new System.Drawing.Point(265, 45);
            this.execute_button.Name = "execute_button";
            this.execute_button.Size = new System.Drawing.Size(84, 30);
            this.execute_button.TabIndex = 1;
            this.execute_button.Text = "Execute";
            this.execute_button.UseVisualStyleBackColor = true;
            this.execute_button.Click += new System.EventHandler(this.execute_button_Click);
            this.execute_button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.Controls.Add(this.execute_button);
            this.Controls.Add(this.error_label);
            this.Controls.Add(this.command_input);

        }

        private void initMemoryElements()
        {
            const int scale = 30;
            const int offsetX = 450;
            const int offsetY = 50;

            makeTitle("Memory", offsetX, offsetY - 40);

            for (int i = 0; i < memoryWidth * memoryWidth; i++)
            {
                int x = i % memoryWidth;
                int y = (int)Math.Floor((double)i / memoryWidth);

                Label l = new System.Windows.Forms.Label();
                l.Location = new System.Drawing.Point(x * scale + offsetX, y * scale + offsetY);
                l.Size = new System.Drawing.Size(scale, scale);
                l.Name = $"MemoryBox{i}";
                l.Text = string.Format("{0:X2}", this.microprocessor.memory[this.memoryPage * memoryWidth * memoryWidth + i]);
                l.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                this.memory_label.Add(l);
                this.Controls.Add(l);
            }

            makePagination(offsetX, offsetY, scale);
        }

        private void makePagination(int offsetX, int offsetY, int scale)
        {
            int y = this.memoryWidth * scale + offsetY + 10;
            // 
            // prev_button
            // 
            this.next_button = new System.Windows.Forms.Button();
            this.next_button.Location = new System.Drawing.Point(offsetX + this.memoryWidth * scale - 50, y);
            this.next_button.Name = "next_button";
            this.next_button.Size = new System.Drawing.Size(50, 30);
            this.next_button.TabIndex = 1;
            this.next_button.Text = ">";
            this.next_button.UseVisualStyleBackColor = true;
            this.next_button.Click += new System.EventHandler(this.next_button_Click);
            this.next_button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // next_button
            // 
            this.prev_button = new System.Windows.Forms.Button();
            this.prev_button.Location = new System.Drawing.Point(offsetX, y);
            this.prev_button.Name = "prev_button";
            this.prev_button.Size = new System.Drawing.Size(50, 30);
            this.prev_button.TabIndex = 1;
            this.prev_button.Text = "<";
            this.prev_button.UseVisualStyleBackColor = true;
            this.prev_button.Click += new System.EventHandler(this.prev_button_Click);
            this.prev_button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.prev_button.Enabled = false;


            int pagWidth = 120;
            this.page_label = new System.Windows.Forms.Label();
            this.page_label.Location = new System.Drawing.Point(offsetX + memoryWidth * scale/2 - pagWidth / 2 , y);
            this.page_label.Name = "page_label";
            this.page_label.Size = new System.Drawing.Size(pagWidth, 30);
            this.page_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.page_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.page_label.Font = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);


            this.Controls.Add(this.next_button);
            this.Controls.Add(this.prev_button);
            this.Controls.Add(this.page_label);
        }

        private void initRegistersElements()
        {
            const int scale = 30;
            const int width = 75;
            const int offsetX = 10;
            const int offsetY = 150;
            int count = 0;
            Font font = new Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            makeTitle("Registers", offsetX, offsetY - 40);

            foreach (Register register in this.microprocessor.registers.Values)
            {
                Label v = new System.Windows.Forms.Label();
                v.Location = new System.Drawing.Point(offsetX, scale * count + offsetY);
                v.Size = new System.Drawing.Size(width, scale);
                v.Name = $"RegisterBoxName{register.GetName()}";
                v.Text = register.GetName();
                v.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                v.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                v.Font = font;
                this.Controls.Add(v);

                Label l = new System.Windows.Forms.Label();
                l.Location = new System.Drawing.Point(width + offsetX, scale * count + offsetY);
                l.Size = new System.Drawing.Size(width, scale);
                l.Name = $"RegisterBoxValue{register.GetName()}";
                l.Text = string.Format("{0:X2}", register.GetValue());
                l.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                l.Font = font;
                this.Controls.Add(l);
                this.registers_label.Add(register.GetName(), l);

                count++;
            }
        }

        private void makeTitle(string text, int x, int y, float fontSize = 15F)
        {
            Label title = new System.Windows.Forms.Label();
            title.Location = new System.Drawing.Point(x, y);
            title.AutoSize = true;
            title.Name = $"{text}Title";
            title.Text = text;
            title.Font = new Font("Segoe UI", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Controls.Add(title);
        }

        private void updateUI()
        {
            for (int i = 0; i < memoryWidth * memoryWidth; i++)
            {
                Label l = this.memory_label[i];

                l.Text = string.Format("{0:X2}", this.microprocessor.memory[this.memoryPage * memoryWidth * memoryWidth + i]);
            }

            foreach (string name in this.registers_label.Keys)
            {
                Label label = this.registers_label[name];
                label.Text = string.Format("{0:X2}", this.microprocessor.registers[name].GetValue());
            }

            this.page_label.Text = $"{this.memoryPage + 1} / {this.maxPage + 1}";
        }

        private void execute_button_Click(object sender, EventArgs e)
        {
            try
            {
                this.microprocessor.ReadCommand(command_input.Text);
                this.error_label.Text = "";
            }
            catch (Exception err)
            {
                this.error_label.Text = err.Message;
            }
            finally
            {
                this.updateUI();
            }
        }
        private void prev_button_Click(object sender, EventArgs e)
        {
            if(this.memoryPage - 1 == 0)
            {
                this.prev_button.Enabled = false;
            }
            this.memoryPage--;
            this.next_button.Enabled = true;

            updateUI();
        }
        private void next_button_Click(object sender, EventArgs e)
        {
            if (memoryPage + 1 == this.maxPage)
            {
                this.next_button.Enabled = false;
            }

            this.memoryPage++;
            this.prev_button.Enabled = true;

            updateUI();
        }
    }
}