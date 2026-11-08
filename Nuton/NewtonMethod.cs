using System;
using System.Collections.Generic;

namespace Nuton {
  public class NewtonMethod {
    private const double derivativeStep = 0.000001;
    private const double derivativeDivider = 2.0;
    private const double intervalDivider = 2.0;
    private const double secondDerivativeTolerance = 0.0000000001;
    private const int maxIterations = 100;

    private readonly FormulaEvaluator evaluator;
    private readonly double epsilon;

    public NewtonMethod(FormulaEvaluator evaluator, double epsilon) {
      this.evaluator = evaluator;
      this.epsilon = epsilon;
    }

    public List<IterationStep> FindMinimum(double left, double right) {
      List<IterationStep> steps;
      steps = new List<IterationStep>();

      double current;
      current = (left + right) / intervalDivider;

      int iteration;
      iteration = 0;

      while (iteration < maxIterations) {
        double firstDerivative;
        firstDerivative = CalculateFirstDerivative(current);

        double secondDerivative;
        secondDerivative = CalculateSecondDerivative(current);

        if (Math.Abs(secondDerivative) < secondDerivativeTolerance) {
          throw new InvalidOperationException(
              "Вторая производная близка к нулю, метод Ньютона не применим.");
        }

        double next;
        next = current - firstDerivative / secondDerivative;

        if (double.IsNaN(next) || double.IsInfinity(next)) {
          throw new InvalidOperationException(
              "Метод Ньютона расходится: получено недопустимое значение.");
        }

        IterationStep step;
        step = new IterationStep();
        step.Number = iteration;
        step.X = next;
        step.Y = evaluator.Evaluate(next);
        steps.Add(step);

        if (Math.Abs(next - current) < epsilon) {
          return steps;
        }

        current = next;
        iteration++;
      }

      throw new InvalidOperationException(
          "Метод Ньютона не сошёлся за " + maxIterations + " итераций.");
    }

    private double CalculateFirstDerivative(double x) {
      double rightValue;
      rightValue = evaluator.Evaluate(x + derivativeStep);

      double leftValue;
      leftValue = evaluator.Evaluate(x - derivativeStep);

      double result;
      result = (rightValue - leftValue) / (derivativeDivider * derivativeStep);
      return result;
    }

    private double CalculateSecondDerivative(double x) {
      double rightValue;
      rightValue = CalculateFirstDerivative(x + derivativeStep);

      double leftValue;
      leftValue = CalculateFirstDerivative(x - derivativeStep);

      double result;
      result = (rightValue - leftValue) / (derivativeDivider * derivativeStep);
      return result;
    }
  }
}