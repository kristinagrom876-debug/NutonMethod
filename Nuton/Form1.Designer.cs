using System.Drawing;
using System.Windows.Forms;

namespace Nuton {
  partial class Form1 {
    private System.ComponentModel.IContainer components = null;

    private MenuStrip menuStrip;
    private ToolStripMenuItem menuItemActions;
    private ToolStripMenuItem menuItemCalculate;
    private ToolStripMenuItem menuItemStepByStep;
    private ToolStripMenuItem menuItemClear;

    private FlowLayoutPanel inputPanel;
    private Label labelA;
    private TextBox textBoxA;
    private Label labelB;
    private TextBox textBoxB;
    private Label labelEpsilon;
    private TextBox textBoxEpsilon;
    private Label labelFormula;
    private TextBox textBoxFormula;

    private Button buttonNextStep;
    private Label resultLabel;
    private PictureBox chartPictureBox;
    private Label statusLabel;

    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent() {
      this.menuStrip = new MenuStrip();
      this.menuItemActions = new ToolStripMenuItem();
      this.menuItemCalculate = new ToolStripMenuItem();
      this.menuItemStepByStep = new ToolStripMenuItem();
      this.menuItemClear = new ToolStripMenuItem();

      this.inputPanel = new FlowLayoutPanel();
      this.labelA = new Label();
      this.textBoxA = new TextBox();
      this.labelB = new Label();
      this.textBoxB = new TextBox();
      this.labelEpsilon = new Label();
      this.textBoxEpsilon = new TextBox();
      this.labelFormula = new Label();
      this.textBoxFormula = new TextBox();

      this.buttonNextStep = new Button();
      this.resultLabel = new Label();
      this.chartPictureBox = new PictureBox();
      this.statusLabel = new Label();

      this.menuStrip.SuspendLayout();
      this.inputPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chartPictureBox)).BeginInit();
      this.SuspendLayout();

      // menuStrip
      this.menuStrip.BackColor = Color.MidnightBlue;
      this.menuStrip.ForeColor = Color.White;
      this.menuStrip.Font = new Font("Segoe UI", fontSize);

      this.menuItemActions.Text = "Действия";
      this.menuItemActions.ForeColor = Color.White;

      this.menuItemCalculate.Text = "Рассчитать";
      this.menuItemCalculate.Click += new System.EventHandler(this.OnCalculateClick);

      this.menuItemStepByStep.Text = "Пошаговый режим";
      this.menuItemStepByStep.Click += new System.EventHandler(this.OnStepByStepClick);

      this.menuItemClear.Text = "Очистить";
      this.menuItemClear.Click += new System.EventHandler(this.OnClearClick);

      this.menuItemActions.DropDownItems.Add(this.menuItemCalculate);
      this.menuItemActions.DropDownItems.Add(this.menuItemStepByStep);
      this.menuItemActions.DropDownItems.Add(this.menuItemClear);
      this.menuStrip.Items.Add(this.menuItemActions);

      // inputPanel
      this.inputPanel.BackColor = Color.MidnightBlue;
      this.inputPanel.Dock = DockStyle.Top;
      this.inputPanel.AutoSize = true;
      this.inputPanel.Padding = new Padding(0, paddingSmall, 0, paddingSmall);

      // labelA
      this.labelA.AutoSize = true;
      this.labelA.ForeColor = Color.White;
      this.labelA.Font = new Font("Segoe UI", fontSize);
      this.labelA.Text = "a =";

      // textBoxA
      this.textBoxA.Width = inputTextBoxWidth;
      this.textBoxA.Font = new Font("Segoe UI", fontSize);

      // labelB
      this.labelB.AutoSize = true;
      this.labelB.ForeColor = Color.White;
      this.labelB.Font = new Font("Segoe UI", fontSize);
      this.labelB.Text = "b =";

      // textBoxB
      this.textBoxB.Width = inputTextBoxWidth;
      this.textBoxB.Font = new Font("Segoe UI", fontSize);

      // labelEpsilon
      this.labelEpsilon.AutoSize = true;
      this.labelEpsilon.ForeColor = Color.White;
      this.labelEpsilon.Font = new Font("Segoe UI", fontSize);
      this.labelEpsilon.Text = "e =";

      // textBoxEpsilon
      this.textBoxEpsilon.Width = inputTextBoxWidth;
      this.textBoxEpsilon.Font = new Font("Segoe UI", fontSize);

      // labelFormula
      this.labelFormula.AutoSize = true;
      this.labelFormula.ForeColor = Color.White;
      this.labelFormula.Font = new Font("Segoe UI", fontSize);
      this.labelFormula.Text = "f(x) =";

      // textBoxFormula
      this.textBoxFormula.Width = formulaTextBoxWidth;
      this.textBoxFormula.Font = new Font("Segoe UI", fontSize);

      // buttonNextStep
      this.buttonNextStep.Text = "Следующий шаг >>";
      this.buttonNextStep.Width = nextStepButtonWidth;
      this.buttonNextStep.Height = nextStepButtonHeight;
      this.buttonNextStep.BackColor = Color.DarkOrange;
      this.buttonNextStep.ForeColor = Color.White;
      this.buttonNextStep.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
      this.buttonNextStep.Margin = new Padding(paddingSmall * 4, 0, 0, 0);
      this.buttonNextStep.Enabled = false;
      this.buttonNextStep.Click += new System.EventHandler(this.OnNextStepClick);

      this.inputPanel.Controls.Add(this.labelA);
      this.inputPanel.Controls.Add(this.textBoxA);
      this.inputPanel.Controls.Add(this.labelB);
      this.inputPanel.Controls.Add(this.textBoxB);
      this.inputPanel.Controls.Add(this.labelEpsilon);
      this.inputPanel.Controls.Add(this.textBoxEpsilon);
      this.inputPanel.Controls.Add(this.labelFormula);
      this.inputPanel.Controls.Add(this.textBoxFormula);
      this.inputPanel.Controls.Add(this.buttonNextStep);

      // resultLabel
      this.resultLabel.Dock = DockStyle.Top;
      this.resultLabel.Height = resultLabelHeight;
      this.resultLabel.TextAlign = ContentAlignment.MiddleLeft;
      this.resultLabel.ForeColor = Color.LimeGreen;
      this.resultLabel.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
      this.resultLabel.Text = "Ответ: ";
      this.resultLabel.Padding = new Padding(paddingSmall, 0, 0, 0);

      // chartPictureBox
      this.chartPictureBox.Dock = DockStyle.Fill;
      this.chartPictureBox.BackColor = Color.WhiteSmoke;
      this.chartPictureBox.Paint += new PaintEventHandler(this.OnChartPaint);

      // statusLabel
      this.statusLabel.Dock = DockStyle.Bottom;
      this.statusLabel.Height = statusLabelHeight;
      this.statusLabel.TextAlign = ContentAlignment.MiddleLeft;
      this.statusLabel.ForeColor = Color.White;
      this.statusLabel.Font = new Font("Segoe UI", fontSize);
      this.statusLabel.Text = "Введите a, b, e и формулу функции, затем нажмите «Рассчитать».";

      // Form1
      this.AutoScaleDimensions = new SizeF(autoScaleSize, autoScaleSize);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(formWidth, formHeight);
      this.BackColor = Color.MidnightBlue;
      this.Font = new Font("Segoe UI", fontSize);
      this.MainMenuStrip = this.menuStrip;
      this.Text = "Метод Ньютона — поиск локального минимума";

      this.Controls.Add(this.chartPictureBox);
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.resultLabel);
      this.Controls.Add(this.inputPanel);
      this.Controls.Add(this.menuStrip);

      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.inputPanel.ResumeLayout(false);
      this.inputPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chartPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}