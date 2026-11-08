using System;
using System.Windows.Forms;

namespace Nuton {
  internal static class Program {
    [STAThread]
    private static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Form1 mainForm;
      mainForm = new Form1();
      Application.Run(mainForm);
    }
  }
}