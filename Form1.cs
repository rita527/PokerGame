using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace PokerGame
{
    public partial class Form1 : Form
    {
        // ── 遊戲狀態 ──────────────────────────────
        private Deck deck = new Deck();
        private List<Card> hand = new List<Card>();
        private int totalMoney = 1000000;
        private int betAmount = 0;
        private bool hasBet = false;
        private bool hasDealt = false;

        // ── UI 控制項 ─────────────────────────────
        private PictureBox[] cardBoxes = new PictureBox[5];
        private CheckBox[]   selectBoxes = new CheckBox[5];
        private TextBox  txtTotalMoney;
        private TextBox  txtBetAmount;
        private Label    lblResult;

        // ══════════════════════════════════════════
        //  建構子
        // ══════════════════════════════════════════
        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        // ══════════════════════════════════════════
        //  UI 建構
        // ══════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "五張撲克牌";
            this.Size = new Size(700, 490);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(34, 100, 60);   // 綠色牌桌

            // ── 牌桌區 ────────────────────────────
            var gbTable = MakeGroupBox("牌桌", 10, 10, 665, 225);
            this.Controls.Add(gbTable);

            for (int i = 0; i < 5; i++)
            {
                int x = 15 + i * 128;
                int idx = i;   // closure 捕捉

                // 牌面 PictureBox
                var pb = new PictureBox
                {
                    Location  = new Point(x, 20),
                    Size      = new Size(110, 158),
                    BackColor = Color.Transparent
                };
                pb.Paint += (s, e) => DrawCard(e.Graphics, idx);
                cardBoxes[i] = pb;
                gbTable.Controls.Add(pb);

                // 換牌勾選框
                var cb = new CheckBox
                {
                    Text      = "選擇換牌",
                    ForeColor = Color.White,
                    Font      = new Font("微軟正黑體", 9f),
                    Location  = new Point(x + 5, 183),
                    Size      = new Size(105, 24)
                };
                cb.CheckedChanged += (s, e) => cardBoxes[idx].Invalidate();
                selectBoxes[i] = cb;
                gbTable.Controls.Add(cb);
            }

            // ── 下注區 ────────────────────────────
            var gbBet = MakeGroupBox("下注", 10, 245, 665, 68);
            this.Controls.Add(gbBet);

            gbBet.Controls.Add(MakeLabel("總資金", 15, 30));

            txtTotalMoney = new TextBox
            {
                Text     = "1,000,000",
                ReadOnly = true,
                Location = new Point(75, 27),
                Size     = new Size(130, 25),
                Font     = new Font("微軟正黑體", 10f)
            };
            gbBet.Controls.Add(txtTotalMoney);

            gbBet.Controls.Add(MakeLabel("押注金額", 235, 30));

            txtBetAmount = new TextBox
            {
                Text     = "500",
                Location = new Point(318, 27),
                Size     = new Size(100, 25),
                Font     = new Font("微軟正黑體", 10f)
            };
            gbBet.Controls.Add(txtBetAmount);

            var btnBet = MakeButton("押注", 435, 25, 75, 30);
            btnBet.BackColor = Color.FromArgb(220, 80, 60);
            btnBet.ForeColor = Color.White;
            btnBet.Click += BtnBet_Click;
            gbBet.Controls.Add(btnBet);

            // ── 功能區 ────────────────────────────
            var gbFunc = MakeGroupBox("功能", 10, 323, 665, 68);
            this.Controls.Add(gbFunc);

            var btnDeal = MakeButton("發牌", 15, 25, 85, 30);
            btnDeal.Click += BtnDeal_Click;
            gbFunc.Controls.Add(btnDeal);

            var btnExchange = MakeButton("換牌", 115, 25, 85, 30);
            btnExchange.Click += BtnExchange_Click;
            gbFunc.Controls.Add(btnExchange);

            var btnJudge = MakeButton("判斷牌型", 215, 25, 105, 30);
            btnJudge.Click += BtnJudge_Click;
            gbFunc.Controls.Add(btnJudge);

            lblResult = new Label
            {
                Text      = "請先押注後發牌",
                ForeColor = Color.LightYellow,
                Font      = new Font("微軟正黑體", 11f, FontStyle.Bold),
                Location  = new Point(335, 28),
                AutoSize  = true
            };
            gbFunc.Controls.Add(lblResult);
        }

        // ── Helper：建立 GroupBox ─────────────────
        private GroupBox MakeGroupBox(string text, int x, int y, int w, int h)
        {
            return new GroupBox
            {
                Text      = text,
                ForeColor = Color.White,
                Font      = new Font("微軟正黑體", 10f),
                Location  = new Point(x, y),
                Size      = new Size(w, h),
                BackColor = Color.Transparent
            };
        }

        // ── Helper：建立 Label ────────────────────
        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text      = text,
                ForeColor = Color.White,
                Font      = new Font("微軟正黑體", 10f),
                Location  = new Point(x, y),
                AutoSize  = true
            };
        }

        // ── Helper：建立 Button ───────────────────
        private Button MakeButton(string text, int x, int y, int w, int h)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("微軟正黑體", 10f),
                ForeColor = Color.Black,
                UseVisualStyleBackColor = true
            };
        }

        // ══════════════════════════════════════════
        //  繪製牌面
        // ══════════════════════════════════════════
        private void DrawCard(Graphics g, int index)
        {
            var box = cardBoxes[index];
            int w = box.Width;
            int h = box.Height;

            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.AntiAlias;

            bool selected  = selectBoxes[index].Checked;
            Color bgColor  = selected ? Color.FromArgb(255, 252, 200) : Color.White;
            Color border   = selected ? Color.Gold  : Color.Silver;
            float bw       = selected ? 3f : 1.5f;

            // 圓角矩形背景
            using (var path = RoundedRect(new Rectangle(1, 1, w - 2, h - 2), 10))
            {
                using (var bg = new SolidBrush(bgColor))
                    g.FillPath(bg, path);
                using (var pen = new Pen(border, bw))
                    g.DrawPath(pen, path);
            }

            // 尚未發牌時：顯示牌背
            if (index >= hand.Count)
            {
                DrawCardBack(g, w, h);
                return;
            }

            var card = hand[index];
            Color cc = card.IsRed ? Color.Crimson : Color.Black;

            using var brush    = new SolidBrush(cc);
            using var fontRank = new Font("Arial", 15f, FontStyle.Bold);
            using var fontSuit = new Font("Arial", 12f);
            using var fontBig  = new Font("Arial", 44f);

            string rs = card.RankString;
            string ss = card.SuitSymbol;

            // 左上：點數 + 花色
            g.DrawString(rs, fontRank, brush, new PointF(5f, 3f));
            SizeF rsz = g.MeasureString(rs, fontRank);
            g.DrawString(ss, fontSuit, brush, new PointF(7f, rsz.Height));

            // 中央大花色
            SizeF bsz = g.MeasureString(ss, fontBig);
            g.DrawString(ss, fontBig, brush,
                new PointF((w - bsz.Width) / 2f + 2f, (h - bsz.Height) / 2f));

            // 右下：旋轉 180° 重複繪製
            var state = g.Save();
            g.TranslateTransform(w, h);
            g.RotateTransform(180f);
            g.DrawString(rs, fontRank, brush, new PointF(5f, 3f));
            g.DrawString(ss, fontSuit, brush, new PointF(7f, rsz.Height));
            g.Restore(state);
        }

        // ── 牌背圖案 ──────────────────────────────
        private void DrawCardBack(Graphics g, int w, int h)
        {
            using (var path = RoundedRect(new Rectangle(1, 1, w - 2, h - 2), 10))
            using (var bg = new LinearGradientBrush(
                new Point(0, 0), new Point(w, h),
                Color.FromArgb(30, 30, 120), Color.FromArgb(80, 80, 200)))
            {
                g.FillPath(bg, path);
            }

            // 菱形格紋
            using var pen = new Pen(Color.FromArgb(80, 255, 255, 255), 1f);
            for (int y = -h; y < h * 2; y += 12)
            {
                g.DrawLine(pen, 0, y, w, y + w);
                g.DrawLine(pen, 0, y, w, y - w);
            }

            // 中央文字
            using var f = new Font("微軟正黑體", 11f, FontStyle.Bold);
            using var b = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
            SizeF sz = g.MeasureString("🂠", f);
            g.DrawString("♦♦", f, b,
                new PointF((w - g.MeasureString("♦♦", f).Width) / 2f,
                           (h - g.MeasureString("♦♦", f).Height) / 2f));
        }

        // ── 圓角矩形路徑 ─────────────────────────
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X,              bounds.Y,               d, d, 180, 90);
            path.AddArc(bounds.Right - d,      bounds.Y,               d, d, 270, 90);
            path.AddArc(bounds.Right - d,      bounds.Bottom - d,      d, d, 0,   90);
            path.AddArc(bounds.X,              bounds.Bottom - d,      d, d, 90,  90);
            path.CloseFigure();
            return path;
        }

        // ══════════════════════════════════════════
        //  按鈕事件
        // ══════════════════════════════════════════

        // ── 押注 ──────────────────────────────────
        private void BtnBet_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBetAmount.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show("請輸入有效的押注金額（正整數）！",
                    "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (amount > totalMoney)
            {
                MessageBox.Show($"押注金額 {amount:N0} 超過總資金 {totalMoney:N0}！",
                    "資金不足", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (hasBet)
            {
                MessageBox.Show("本局已押注，請先發牌後再押注。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            betAmount   = amount;
            totalMoney -= betAmount;
            txtTotalMoney.Text = totalMoney.ToString("N0");
            hasBet      = true;
            lblResult.Text = $"已押注 {betAmount:N0} 元，請發牌！";
        }

        // ── 發牌 ──────────────────────────────────
        private void BtnDeal_Click(object sender, EventArgs e)
        {
            deck.Reset();
            deck.Shuffle();
            hand.Clear();

            for (int i = 0; i < 5; i++)
                hand.Add(deck.Draw());

            foreach (var cb in selectBoxes) cb.Checked = false;
            foreach (var pb in cardBoxes)   pb.Invalidate();

            hasDealt = true;

            if (!hasBet)
                lblResult.Text = "已發牌（未押注）";
            else
                lblResult.Text = $"已押注 {betAmount:N0} 元，可換牌或直接判斷";
        }

        // ── 換牌 ──────────────────────────────────
        private void BtnExchange_Click(object sender, EventArgs e)
        {
            if (!hasDealt)
            {
                MessageBox.Show("請先發牌！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!selectBoxes.Any(cb => cb.Checked))
            {
                MessageBox.Show("請勾選要換的牌！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                if (selectBoxes[i].Checked)
                {
                    hand[i] = deck.Draw();
                    selectBoxes[i].Checked = false;
                }
            }

            foreach (var pb in cardBoxes) pb.Invalidate();
        }

        // ── 判斷牌型 ──────────────────────────────
        private void BtnJudge_Click(object sender, EventArgs e)
        {
            if (!hasDealt || hand.Count < 5)
            {
                MessageBox.Show("請先發牌！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var    result     = HandEvaluator.Evaluate(hand);
            string handName   = HandEvaluator.GetHandName(result);
            int    multiplier = HandEvaluator.GetMultiplier(result);

            string msg;

            if (hasBet)
            {
                if (multiplier > 0)
                {
                    int win = betAmount * multiplier;
                    totalMoney += win;
                    txtTotalMoney.Text = totalMoney.ToString("N0");
                    msg = $"🎉 恭喜！\n\n牌型：{handName}\n賠率：{multiplier}x\n\n"
                        + $"押注：{betAmount:N0} 元\n贏得：{win:N0} 元\n"
                        + $"目前總資金：{totalMoney:N0} 元";
                }
                else
                {
                    msg = $"😢 很遺憾，沒有中獎！\n\n牌型：{handName}\n\n"
                        + $"損失：{betAmount:N0} 元\n目前總資金：{totalMoney:N0} 元";
                }
                hasBet = false;
            }
            else
            {
                msg = $"牌型：{handName}\n"
                    + (multiplier > 0
                        ? $"賠率：{multiplier}x（本局未押注，不計算金額）"
                        : "無特殊牌型（未押注）");
            }

            lblResult.Text = multiplier > 0
                ? $"{handName}  {multiplier}x"
                : handName;

            MessageBoxIcon icon = multiplier > 0
                ? MessageBoxIcon.Information
                : MessageBoxIcon.None;

            MessageBox.Show(msg, "判斷結果", MessageBoxButtons.OK, icon);
        }
    }
}
