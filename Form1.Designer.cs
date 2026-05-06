using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PokerGame
{
    partial class Form1
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(684, 420);
            this.Name = "Form1";
            this.Text = "五張撲克牌";
            this.ResumeLayout(false);
        }
    }
}
