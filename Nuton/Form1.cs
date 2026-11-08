using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Nuton {
  public partial class Form1 : Form {
    // Параметры окна
    private const int formWidth = 1000;
    private const int formHeight = 700;
    private const int inputTextBoxWidth = 120;
    private const int formulaTextBoxWidth = 240;
    private const int statusLabelHeight = 28;
    private const int paddingSmall = 4;
    private const float fontSize = 10f;
    private const float autoScaleSize = 10f;

    // Параметры графика
    private const int chartSampleCount = 400;
    private const int margin = 50;
    private const int pointRadius = 4;
    private const int diamondSize = 8;
    private const int functionPenWidth = 3;
    private const int pathPenWidth = 2;

    // Форматирование
    private const string numberFormat = "F6";

    // Состояние
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
      double left;
      double right;
      double epsilon;
      string formula;

      if (!TryReadInput(out left, out right, out epsilon, out formula)) {
        return;
      }

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

        chartPictureBox.Invalidate();

        IterationStep lastStep;
        lastStep = steps[steps.Count - 1];

        statusLabel.Text = "Минимум: x = " + FormatNumber(lastStep.X) +
            ", f(x) = " + FormatNumber(lastStep.Y) +
            ". Итераций: " + steps.Count + ".";
      } catch (Exception exception) {
        ShowError(exception.Message);
      }
    }

    private void OnStepByStepClick(object sender, EventArgs e) {
      double left;
      double right;
      double epsilon;
      string formula;

      if (!TryReadInput(out left, out right, out epsilon, out formula)) {
        return;
      }

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

        chartPictureBox.Invalidate();
        ShowCurrentStep(steps[currentStepIndex]);
        statusLabel.Text = "Пошаговый режим. " + statusLabel.Text;
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
        statusLabel.Text = "Все шаги просмотрены. Нажмите «Очистить» или выполните новый расчёт.";
        return;
      }

      currentStepIndex++;
      chartPictureBox.Invalidate();
      ShowCurrentStep(steps[currentStepIndex]);
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

      chartPictureBox.Invalidate();
      statusLabel.Text = "Данные очищены.";
    }

    // ============================== Отрисовка графика ==============================
    private void OnChartPaint(object sender, PaintEventArgs e) {
      if (!hasChartData || evaluator == null) {
        return;
      }

      Graphics graphics;
      graphics = e.Graphics;
      graphics.SmoothingMode = SmoothingMode.AntiAlias;

      int width;
      width = chartPictureBox.Width;
      int height;
      height = chartPictureBox.Height;

      if (width <= 2 * margin || height <= 2 * margin) {
        return;
      }

      // 1. Находим минимум и максимум Y для масштабирования
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
          if (y < minY) {
            minY = y;
          }
          if (y > maxY) {
            maxY = y;
          }
        } catch {
          // Пропускаем недопустимые точки
        }
        sampleIndex++;
      }

      if (minY == double.MaxValue || maxY == double.MinValue) {
        return;
      }

      // Отступы по Y
      double rangeY;
      rangeY = maxY - minY;
      if (rangeY == 0) {
        rangeY = 1;
      }
      minY = minY - rangeY * 0.1;
      maxY = maxY + rangeY * 0.1;
      rangeY = maxY - minY;

      // 2. Рисуем оси координат
      Pen axisPen;
      axisPen = new Pen(Color.Black, 1);
      graphics.DrawLine(axisPen, margin, height - margin, width - margin, height - margin);
      graphics.DrawLine(axisPen, margin, margin, margin, height - margin);

      // 3. Рисуем саму функцию
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

          float pixelX;
          pixelX = (float)(margin + (x - currentLeft) / (currentRight - currentLeft) * (width - 2 * margin));
          float pixelY;
          pixelY = (float)(height - margin - (y - minY) / rangeY * (height - 2 * margin));

          PointF currentPoint;
          currentPoint = new PointF(pixelX, pixelY);

          if (previousPoint.HasValue) {
            graphics.DrawLine(functionPen, previousPoint.Value, currentPoint);
          }
          previousPoint = currentPoint;
        } catch {
          previousPoint = null;
        }
        sampleIndex++;
      }

      // 4. Рисуем точки метода Ньютона (траекторию)
      Pen pathPen;
      pathPen = new Pen(Color.DarkOrange, pathPenWidth);
      Brush pointBrush;
      pointBrush = new SolidBrush(Color.DarkOrange);

      int stepIndex;
      stepIndex = 0;
      while (stepIndex <= currentStepIndex) {
        IterationStep step;
        step = steps[stepIndex];

        float pixelX;
        pixelX = (float)(margin + (step.X - currentLeft) / (currentRight - currentLeft) * (width - 2 * margin));
        float pixelY;
        pixelY = (float)(height - margin - (step.Y - minY) / rangeY * (height - 2 * margin));

        PointF currentPoint;
        currentPoint = new PointF(pixelX, pixelY);

        if (stepIndex > 0) {
          IterationStep previousStep;
          previousStep = steps[stepIndex - 1];

          float prevPixelX;
          prevPixelX = (float)(margin + (previousStep.X - currentLeft) / (currentRight - currentLeft) * (width - 2 * margin));
          float prevPixelY;
          prevPixelY = (float)(height - margin - (previousStep.Y - minY) / rangeY * (height - 2 * margin));

          graphics.DrawLine(pathPen, prevPixelX, prevPixelY, pixelX, pixelY);
        }

        graphics.FillEllipse(pointBrush, pixelX - pointRadius, pixelY - pointRadius, pointRadius * 2, pointRadius * 2);
        stepIndex++;
      }

      // 5. Рисуем текущую точку (красный ромб)
      if (currentStepIndex >= 0 && currentStepIndex < steps.Count) {
        IterationStep step;
        step = steps[currentStepIndex];

        float pixelX;
        pixelX = (float)(margin + (step.X - currentLeft) / (currentRight - currentLeft) * (width - 2 * margin));
        float pixelY;
        pixelY = (float)(height - margin - (step.Y - minY) / rangeY * (height - 2 * margin));

        Brush currentBrush;
        currentBrush = new SolidBrush(Color.Red);

        PointF[] diamondPoints;
        diamondPoints = new PointF[4];
        diamondPoints[0] = new PointF(pixelX, pixelY - diamondSize);
        diamondPoints[1] = new PointF(pixelX + diamondSize, pixelY);
        diamondPoints[2] = new PointF(pixelX, pixelY + diamondSize);
        diamondPoints[3] = new PointF(pixelX - diamondSize, pixelY);

        graphics.FillPolygon(currentBrush, diamondPoints);
      }
    }

    // ============================== Ввод и обработка ошибок ==============================
    private void ShowCurrentStep(IterationStep step) {
      statusLabel.Text = "Шаг " + step.Number +
          ": x = " + FormatNumber(step.X) +
          ", f(x) = " + FormatNumber(step.Y) + ".";
    }

    private bool TryReadInput(out double left, out double right, out double epsilon, out string formula) {
      left = 0;
      right = 0;
      epsilon = 0;
      formula = string.Empty;

      if (!TryParseNumber(textBoxA.Text, out left)) {
        ShowError("Параметр a должен быть вещественным числом.");
        return false;
      }

      if (!TryParseNumber(textBoxB.Text, out right)) {
        ShowError("Параметр b должен быть вещественным числом.");
        return false;
      }

      if (!TryParseNumber(textBoxEpsilon.Text, out epsilon)) {
        ShowError("Точность e должна быть вещественным числом.");
        return false;
      }

      if (left >= right) {
        ShowError("Должно выполняться условие a < b.");
        return false;
      }

      if (epsilon <= 0) {
        ShowError("Точность e должна быть больше нуля.");
        return false;
      }

      formula = textBoxFormula.Text.Trim();
      if (formula.Length == 0) {
        ShowError("Введите формулу функции f(x).");
        return false;
      }

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