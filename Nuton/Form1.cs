using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Nuton {
  public partial class Form1 : Form {
    private const int formWidth = 1000;
    private const int formHeight = 700;
    private const int inputTextBoxWidth = 120;
    private const int formulaTextBoxWidth = 200;
    private const int statusLabelHeight = 28;
    private const int resultLabelHeight = 35;
    private const int paddingSmall = 4;
    private const float fontSize = 10f;
    private const float autoScaleSize = 10f;

    private const int nextStepButtonWidth = 150;
    private const int nextStepButtonHeight = 26;

    private const int chartSampleCount = 400;
    private const int marginLeft = 60;
    private const int marginRight = 30;
    private const int marginTop = 30;
    private const int marginBottom = 50;
    private const int pointRadius = 4;
    private const int pointDiameter = 8;
    private const int diamondSize = 8;
    private const int answerSize = 14;
    private const int functionPenWidth = 3;
    private const int pathPenWidth = 2;
    private const int projectionPenWidth = 1;
    private const int tangentPenWidth = 2;
    private const int tickLength = 6;
    private const int labelOffset = 20;
    private const int stepTextOffsetX = 6;
    private const int stepTextOffsetY = 18;
    private const int targetTicks = 10;
    private const int formulaOffsetX = 10;
    private const int formulaOffsetY = 10;
    private const int axisLabelXOffset = 5;
    private const int axisLabelYOffset = 10;
    private const int axisLabelXMinusOffset = 15;
    private const int axisLabelYMinusOffset = 20;
    private const int boxMargin = 10;

    private const double derivativeStep = 0.000001;
    private const double paddingPercent = 0.1;
    private const double defaultRange = 1.0;
    private const double derivativeDivider = 2.0;
    private const double halfSizeDivider = 2.0;

    private const string numberFormat = "F6";
    private const string axisFormat = "0.###";
    private const string answerFormat = "F4";

    private FormulaEvaluator evaluator;
    private List<IterationStep> steps;
    private int currentStepIndex;

    private double currentLeft;
    private double currentRight;
    private bool hasChartData;

    public Form1() {
      InitializeComponent();
    }

    private void OnCalculateClick(object sender, EventArgs e) {

      if (!TryReadInput(out double left, out double right, out double epsilon, out string formula)) return;

      try {
        evaluator = new FormulaEvaluator(formula);
        evaluator.Evaluate(left);

        NewtonMethod method;
        method = new NewtonMethod(evaluator, epsilon);
        steps = method.FindMinimum(left, right);

        currentLeft = left;
        currentRight = right;
        hasChartData = true;
        currentStepIndex = steps.Count - 1;

        buttonNextStep.Enabled = false;

        chartPictureBox.Invalidate();

        IterationStep lastStep;
        lastStep = steps[steps.Count - 1];

        resultLabel.Text = "Ответ: Минимум в точке x = " + FormatNumber(lastStep.X) +
            " | f(x) = " + FormatNumber(lastStep.Y) +
            " | Шагов: " + steps.Count;

        statusLabel.Text = "Расчет завершен успешно.";
      } catch (Exception exception) {
        ShowError(exception.Message);
      }
    }

    private void OnStepByStepClick(object sender, EventArgs e) {

      if (!TryReadInput(out double left, out double right, out double epsilon, out string formula)) return;

      try {
        evaluator = new FormulaEvaluator(formula);
        evaluator.Evaluate(left);

        NewtonMethod method;
        method = new NewtonMethod(evaluator, epsilon);
        steps = method.FindMinimum(left, right);

        currentLeft = left;
        currentRight = right;
        hasChartData = true;
        currentStepIndex = 0;

        buttonNextStep.Enabled = true;

        resultLabel.Text = "Ответ: Пошаговый режим";
        chartPictureBox.Invalidate();
        ShowCurrentStep(steps[currentStepIndex]);
        statusLabel.Text = "Пошаговый режим. Нажимайте кнопку «Следующий шаг».";
      } catch (Exception exception) {
        ShowError(exception.Message);
      }
    }

    private void OnNextStepClick(object sender, EventArgs e) {
      if (steps == null || !hasChartData) {
        ShowError("Сначала выполните расчёт или запустите пошаговый режим.");
        return;
      }

      if (currentStepIndex + 1 >= steps.Count) {
        IterationStep lastStep;
        lastStep = steps[steps.Count - 1];

        resultLabel.Text = "Ответ: Минимум в точке x = " + FormatNumber(lastStep.X) +
            " | f(x) = " + FormatNumber(lastStep.Y) +
            " | Шагов: " + steps.Count;
        statusLabel.Text = "Все шаги просмотрены.";
        buttonNextStep.Enabled = false;
        return;
      }

      ++currentStepIndex;
      chartPictureBox.Invalidate();
      ShowCurrentStep(steps[currentStepIndex]);

      if (currentStepIndex == steps.Count - 1) {
        IterationStep lastStep;
        lastStep = steps[steps.Count - 1];

        resultLabel.Text = "Ответ: Минимум в точке x = " + FormatNumber(lastStep.X) +
            " | f(x) = " + FormatNumber(lastStep.Y) +
            " | Шагов: " + steps.Count;
        buttonNextStep.Enabled = false;
      }
    }

    private void OnClearClick(object sender, EventArgs e) {
      textBoxA.Clear();
      textBoxB.Clear();
      textBoxEpsilon.Clear();
      textBoxFormula.Clear();

      steps = null;
      currentStepIndex = 0;
      evaluator = null;
      hasChartData = false;

      buttonNextStep.Enabled = false;

      resultLabel.Text = "Ответ: ";
      chartPictureBox.Invalidate();
      statusLabel.Text = "Данные очищены.";
    }

    private void OnChartPaint(object sender, PaintEventArgs e) {
      if (!hasChartData || evaluator == null) return;

      Graphics graphics;
      graphics = e.Graphics;
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      int width;
      width = chartPictureBox.Width;
      int height;
      height = chartPictureBox.Height;

      if (width <= marginLeft + marginRight || height <= marginTop + marginBottom) return;

      double minY;
      minY = double.MaxValue;
      double maxY;
      maxY = double.MinValue;

      int sampleIndex;
      sampleIndex = 0;
      while (sampleIndex <= chartSampleCount) {
        double x;
        x = currentLeft + (currentRight - currentLeft) * sampleIndex / chartSampleCount;
        try {
          double y;
          y = evaluator.Evaluate(x);
          if (y < minY) minY = y;
          if (y > maxY) maxY = y;
        } catch { }
        ++sampleIndex;
      }

      if (minY == double.MaxValue || maxY == double.MinValue) return;

      double rangeY;
      rangeY = maxY - minY;
      if (rangeY == 0) rangeY = defaultRange;

      minY -= rangeY * paddingPercent;
      maxY += rangeY * paddingPercent;
      rangeY = maxY - minY;

      Font axisFont;
      axisFont = new Font("Segoe UI", 9f);
      Font stepFont;
      stepFont = new Font("Segoe UI", 9f, FontStyle.Bold);
      Font answerFont;
      answerFont = new Font("Segoe UI", 9f, FontStyle.Bold);
      Font formulaFont;
      formulaFont = new Font("Segoe UI", 12f, FontStyle.Bold | FontStyle.Italic);

      Brush axisBrush;
      axisBrush = new SolidBrush(Color.Black);

      Pen gridPen;
      gridPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dash };

      Pen realAxisPen;
      realAxisPen = new Pen(Color.Black, 2);

      float originX;
      originX = marginLeft;
      float originY;
      originY = height - marginBottom;

      float zeroX;
      zeroX = (float)(originX + (0 - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
      float zeroY;
      zeroY = (float)(originY - (0 - minY) / rangeY * (height - marginTop - marginBottom));

      float axisXPos;
      axisXPos = Math.Max(originX, Math.Min(zeroX, width - marginRight));
      float axisYPos;
      axisYPos = Math.Max(marginTop, Math.Min(zeroY, originY));

      double stepX;
      stepX = GetNiceStep(currentRight - currentLeft, targetTicks);
      double startX;
      startX = Math.Ceiling(currentLeft / stepX) * stepX;

      double xTick;
      xTick = startX;
      while (xTick <= currentRight) {
        float px;
        px = (float)(originX + (xTick - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
        graphics.DrawLine(gridPen, px, marginTop, px, originY);
        graphics.DrawLine(realAxisPen, px, axisYPos, px, axisYPos + tickLength);

        string label;
        label = xTick.ToString(axisFormat, CultureInfo.InvariantCulture);
        SizeF labelSize;
        labelSize = graphics.MeasureString(label, axisFont);
        graphics.DrawString(label, axisFont, axisBrush, (float)(px - labelSize.Width / halfSizeDivider), axisYPos + tickLength + 2);
        xTick += stepX;
      }

      double stepY;
      stepY = GetNiceStep(rangeY, targetTicks);
      double startY;
      startY = Math.Ceiling(minY / stepY) * stepY;

      double yTick;
      yTick = startY;
      while (yTick <= maxY) {
        float py;
        py = (float)(originY - (yTick - minY) / rangeY * (height - marginTop - marginBottom));
        graphics.DrawLine(gridPen, originX, py, width - marginRight, py);
        graphics.DrawLine(realAxisPen, axisXPos - tickLength, py, axisXPos, py);

        string label;
        label = yTick.ToString(axisFormat, CultureInfo.InvariantCulture);
        SizeF labelSize;
        labelSize = graphics.MeasureString(label, axisFont);
        graphics.DrawString(label, axisFont, axisBrush, axisXPos - tickLength - labelSize.Width - 2, (float)(py - labelSize.Height / halfSizeDivider));
        yTick += stepY;
      }

      graphics.DrawLine(realAxisPen, originX, axisYPos, width - marginRight, axisYPos);
      graphics.DrawLine(realAxisPen, axisXPos, marginTop, axisXPos, originY);
      graphics.DrawString("x", new Font("Segoe UI", 12f, FontStyle.Bold), axisBrush, width - marginRight + axisLabelXOffset, axisYPos - axisLabelYOffset);
      graphics.DrawString("y", new Font("Segoe UI", 12f, FontStyle.Bold), axisBrush, axisXPos - axisLabelXMinusOffset, marginTop - axisLabelYMinusOffset);

      Pen functionPen;
      functionPen = new Pen(Color.DeepSkyBlue, functionPenWidth);
      PointF? previousPoint;
      previousPoint = null;

      sampleIndex = 0;
      while (sampleIndex <= chartSampleCount) {
        double x;
        x = currentLeft + (currentRight - currentLeft) * sampleIndex / chartSampleCount;
        try {
          double y;
          y = evaluator.Evaluate(x);
          float px;
          px = (float)(originX + (x - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
          float py;
          py = (float)(originY - (y - minY) / rangeY * (height - marginTop - marginBottom));
          PointF currentPoint;
          currentPoint = new PointF(px, py);

          if (previousPoint.HasValue)
            graphics.DrawLine(functionPen, previousPoint.Value, currentPoint);

          previousPoint = currentPoint;
        } catch { previousPoint = null; }
        ++sampleIndex;
      }

      string formulaText;
      formulaText = "f(x) = " + textBoxFormula.Text;
      graphics.DrawString(formulaText, formulaFont, Brushes.DarkBlue, marginLeft + formulaOffsetX, marginTop + formulaOffsetY);

      Pen pathPen;
      pathPen = new Pen(Color.Orange, pathPenWidth) { DashStyle = DashStyle.Dot };

      Brush pointBrush;
      pointBrush = new SolidBrush(Color.Orange);

      int i;
      i = 0;
      while (i <= currentStepIndex) {
        IterationStep step;
        step = steps[i];
        float px;
        px = (float)(originX + (step.X - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
        float py;
        py = (float)(originY - (step.Y - minY) / rangeY * (height - marginTop - marginBottom));

        if (i > 0) {
          IterationStep prevStep;
          prevStep = steps[i - 1];
          float prevPx;
          prevPx = (float)(originX + (prevStep.X - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
          float prevPy;
          prevPy = (float)(originY - (prevStep.Y - minY) / rangeY * (height - marginTop - marginBottom));
          graphics.DrawLine(pathPen, prevPx, prevPy, px, py);
        }

        graphics.FillEllipse(pointBrush, px - pointRadius, py - pointRadius, pointDiameter, pointDiameter);

        string stepText;
        stepText = (i + 1).ToString();
        SizeF stepTextSize;
        stepTextSize = graphics.MeasureString(stepText, stepFont);
        graphics.FillRectangle(Brushes.White, px + stepTextOffsetX, py - stepTextOffsetY, stepTextSize.Width, stepTextSize.Height);
        graphics.DrawString(stepText, stepFont, Brushes.DarkRed, px + stepTextOffsetX, py - stepTextOffsetY);
        ++i;
      }

      if (currentStepIndex >= 0 && currentStepIndex < steps.Count) {
        IterationStep step;
        step = steps[currentStepIndex];
        float px;
        px = (float)(originX + (step.X - currentLeft) / (currentRight - currentLeft) * (width - marginLeft - marginRight));
        float py;
        py = (float)(originY - (step.Y - minY) / rangeY * (height - marginTop - marginBottom));

        double yLeft;
        yLeft = evaluator.Evaluate(step.X - derivativeStep);
        double yRight;
        yRight = evaluator.Evaluate(step.X + derivativeStep);
        double slope;
        slope = (yRight - yLeft) / (derivativeDivider * derivativeStep);

        double tangentYLeft;
        tangentYLeft = step.Y + slope * (currentLeft - step.X);
        double tangentYRight;
        tangentYRight = step.Y + slope * (currentRight - step.X);

        float tangentPxLeft;
        tangentPxLeft = originX;
        float tangentPyLeft;
        tangentPyLeft = (float)(originY - (tangentYLeft - minY) / rangeY * (height - marginTop - marginBottom));
        float tangentPxRight;
        tangentPxRight = width - marginRight;
        float tangentPyRight;
        tangentPyRight = (float)(originY - (tangentYRight - minY) / rangeY * (height - marginTop - marginBottom));

        Pen tangentPen;
        tangentPen = new Pen(Color.Magenta, tangentPenWidth) { DashStyle = DashStyle.Dash };
        graphics.DrawLine(tangentPen, tangentPxLeft, tangentPyLeft, tangentPxRight, tangentPyRight);

        Pen projectionPen;
        projectionPen = new Pen(Color.DimGray, projectionPenWidth) { DashStyle = DashStyle.Dash };
        graphics.DrawLine(projectionPen, axisXPos, py, px, py);
        graphics.DrawLine(projectionPen, px, axisYPos, px, py);

        bool isFinalStep;
        isFinalStep = (currentStepIndex == steps.Count - 1);

        if (isFinalStep) {
          float answerRadius;
          answerRadius = (float)(answerSize / halfSizeDivider);

          Brush answerBrush;
          answerBrush = new SolidBrush(Color.LimeGreen);
          graphics.FillEllipse(answerBrush, px - answerRadius, py - answerRadius, answerSize, answerSize);

          string answerX;
          answerX = "x = " + step.X.ToString(answerFormat, CultureInfo.InvariantCulture);
          string answerY;
          answerY = "f(x) = " + step.Y.ToString(answerFormat, CultureInfo.InvariantCulture);
          string answerSteps;
          answerSteps = "Шагов: " + steps.Count;

          SizeF sizeX;
          sizeX = graphics.MeasureString(answerX, answerFont);
          SizeF sizeY;
          sizeY = graphics.MeasureString(answerY, answerFont);
          SizeF sizeSteps;
          sizeSteps = graphics.MeasureString(answerSteps, answerFont);

          float boxWidth;
          boxWidth = Math.Max(Math.Max(sizeX.Width, sizeY.Width), sizeSteps.Width) + paddingSmall * 2;
          float boxHeight;
          boxHeight = sizeX.Height + sizeY.Height + sizeSteps.Height + paddingSmall * 3;

          float boxX;
          boxX = px + labelOffset;
          float boxY;
          boxY = py - boxHeight - boxMargin;

          if (boxX + boxWidth > width - marginRight) boxX = px - boxWidth - labelOffset;
          if (boxY < marginTop) boxY = py + labelOffset;

          graphics.FillRectangle(Brushes.White, boxX, boxY, boxWidth, boxHeight);
          graphics.DrawRectangle(Pens.DarkGreen, boxX, boxY, boxWidth, boxHeight);

          graphics.DrawString(answerX, answerFont, Brushes.Black, boxX + paddingSmall, boxY + paddingSmall);
          graphics.DrawString(answerY, answerFont, Brushes.Black, boxX + paddingSmall, boxY + paddingSmall + sizeX.Height);
          graphics.DrawString(answerSteps, answerFont, Brushes.Black, boxX + paddingSmall, boxY + paddingSmall + sizeX.Height + sizeY.Height);
        } else {
          Brush currentBrush;
          currentBrush = new SolidBrush(Color.Red);
          PointF[] diamondPoints;
          diamondPoints = new PointF[4];
          diamondPoints[0] = new PointF(px, py - diamondSize);
          diamondPoints[1] = new PointF(px + diamondSize, py);
          diamondPoints[2] = new PointF(px, py + diamondSize);
          diamondPoints[3] = new PointF(px - diamondSize, py);
          graphics.FillPolygon(currentBrush, diamondPoints);
        }
      }
    }

    private double GetNiceStep(double range, int targetTicks) {
      double rawStep;
      rawStep = range / targetTicks;
      double mag;
      mag = Math.Pow(10, Math.Floor(Math.Log10(rawStep)));
      double norm;
      norm = rawStep / mag;
      double niceNorm;
      niceNorm = 1;

      if (norm >= 5) niceNorm = 5;
      else if (norm >= 2) niceNorm = 2;

      return niceNorm * mag;
    }

    private void ShowCurrentStep(IterationStep step) {
      statusLabel.Text = "Шаг " + (step.Number + 1) +
          ": x = " + FormatNumber(step.X) +
          ", f(x) = " + FormatNumber(step.Y) + ".";
    }

    private bool TryReadInput(out double left, out double right, out double epsilon, out string formula) {
      left = 0;
      right = 0;
      epsilon = 0;
      formula = string.Empty;

      if (!TryParseNumber(textBoxA.Text, out left)) { ShowError("Параметр a должен быть вещественным числом."); return false; }
      if (!TryParseNumber(textBoxB.Text, out right)) { ShowError("Параметр b должен быть вещественным числом."); return false; }
      if (!TryParseNumber(textBoxEpsilon.Text, out epsilon)) { ShowError("Точность e должна быть вещественным числом."); return false; }

      if (left >= right) { ShowError("Должно выполняться условие a < b."); return false; }
      if (epsilon <= 0) { ShowError("Точность e должна быть больше нуля."); return false; }

      formula = textBoxFormula.Text.Trim();
      if (formula.Length == 0) { ShowError("Введите формулу функции f(x)."); return false; }

      return true;
    }

    private bool TryParseNumber(string text, out double value) {
      string normalized;
      normalized = text.Trim().Replace(',', '.');
      return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private string FormatNumber(double value) {
      return value.ToString(numberFormat, CultureInfo.InvariantCulture);
    }

    private void ShowError(string message) {
      MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      statusLabel.Text = message;
    }
  }
}