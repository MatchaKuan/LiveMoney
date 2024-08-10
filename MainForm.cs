namespace LiveMoney3
{
    public partial class MainForm : Form
    {
        TextBox[] textBoxes;

        // 計算機
        Double resultValue = 0;
        String operationPerformed = "";
        bool isOperationPerformed = false;

        public MainForm()
        {
            InitializeComponent();
            // 初始化 textBoxes 數組
            textBoxes = new TextBox[] { tbIncome, tbPocketmoney, tbMonthly, tbAnnual, tbHome, tbPublic, tbDream, tbInvestment, tbBackup };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            CalculateAccounts();

            // 啟用 TextChanged 事件的計算，以便在表單加載完畢後追蹤用戶輸入
            foreach (var textBox in textBoxes)
            {
                textBox.TextChanged += (s, args) => CalculateAccounts();
            }
        }

        private void LoadSettings()
        {
            // 創建一個字典，將 TextBox 控件的名稱對應到 Settings.settings 中的整數型設定值
            var settingsIntMap = new Dictionary<string, int>
            {
                {"tbIncome", Settings.Default.Income},
                {"tbPocketmoney", Settings.Default.M_Pocketmoney},
                {"tbMonthly", Settings.Default.M_Monthly},
                {"tbAnnual", Settings.Default.M_Annual},
                {"tbHome", Settings.Default.M_Home},
                {"tbPublic", Settings.Default.M_Public},
                {"tbDream", Settings.Default.M_Dream},
                {"tbInvestment", Settings.Default.M_Investment},
                {"tbBackup", Settings.Default.M_Backup}
            };

            // 創建另一個字典，將 TextBox 控件的名稱對應到 Settings.settings 中的字符串型設定值
            var settingsStringMap = new Dictionary<string, string>
            {
                {"Bank01", Settings.Default.Bank01},
                {"Bank02", Settings.Default.Bank02},
                {"Bank03", Settings.Default.Bank03},
                {"Bank04", Settings.Default.Bank04},
                {"Bank05", Settings.Default.Bank05},
                {"Bank06", Settings.Default.Bank06},
                {"Bank07", Settings.Default.Bank07},
                {"Bank08", Settings.Default.Bank08}
            };

            // 遍歷 settingsIntMap 字典，將整數型設定值加載到對應的 TextBox 控件中
            foreach (var item in settingsIntMap)
            {
                if (this.Controls.Find(item.Key, true).FirstOrDefault() is TextBox textBox)
                {
                    int value;
                    if (int.TryParse(item.Value.ToString(), out value))
                    {
                        textBox.Text = value.ToString();
                    }
                    else
                    {
                        textBox.Text = "0"; // 或其他合理的預設值
                    }
                }
            }

            // 遍歷 settingsStringMap 字典，將字符串型設定值加載到對應的 TextBox 控件中
            foreach (var item in settingsStringMap)
            {
                if (this.Controls.Find(item.Key, true).FirstOrDefault() is TextBox textBox)
                {
                    textBox.Text = item.Value.ToString();
                }
            }
        }

        // 關閉時儲存設定
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Income = Convert.ToInt32(tbIncome.Text);
            Settings.Default.M_Pocketmoney = Convert.ToInt32(tbPocketmoney.Text);
            Settings.Default.M_Monthly = Convert.ToInt32(tbMonthly.Text);
            Settings.Default.M_Annual = Convert.ToInt32(tbAnnual.Text);
            Settings.Default.M_Home = Convert.ToInt32(tbHome.Text);
            Settings.Default.M_Public = Convert.ToInt32(tbPublic.Text);
            Settings.Default.M_Dream = Convert.ToInt32(tbDream.Text);
            Settings.Default.M_Investment = Convert.ToInt32(tbInvestment.Text);
            Settings.Default.M_Backup = Convert.ToInt32(tbBackup.Text);

            Settings.Default.Bank01 = Bank01.Text;
            Settings.Default.Bank02 = Bank02.Text;
            Settings.Default.Bank03 = Bank03.Text;
            Settings.Default.Bank04 = Bank04.Text;
            Settings.Default.Bank05 = Bank05.Text;
            Settings.Default.Bank06 = Bank06.Text;
            Settings.Default.Bank07 = Bank07.Text;
            Settings.Default.Bank08 = Bank08.Text;

            Settings.Default.Save();
        }

        // 帳戶計算
        private void CalculateAccounts()
        {
            if (TryGetIntValues(out int m_income, out int m_pocketmoney, out int m_monthly, out int m_annual, out int m_home, out int m_public, out int m_dream, out int m_investment, out int m_backup))
            {
                // 年租、公共費，分12個月計算
                int newAnnual = m_annual / 12;
                int newPublic = m_public / 12;

                // 計算剩餘餘額
                int remaining = m_income - m_pocketmoney - m_monthly - newAnnual - m_home - newPublic;

                // 計算總百分比
                int totalPercentage = m_dream + m_investment + m_backup;

                // 確保百分比總和不為0
                if (totalPercentage > 0)
                {
                    // 按百分比分配剩餘金額
                    int dreamAmount = remaining * m_dream / totalPercentage;
                    int investmentAmount = remaining * m_investment / totalPercentage;
                    int backupAmount = remaining * m_backup / totalPercentage;

                    // 顯示結果
                    Result01.Text = m_pocketmoney.ToString(); // 零用金
                    Result02.Text = m_monthly.ToString(); // 月租
                    Result03.Text = newAnnual.ToString(); // 年租
                    Result04.Text = m_home.ToString(); // 家用帳
                    Result05.Text = newPublic.ToString(); // 公共帳

                    Result06.Text = dreamAmount.ToString(); // 夢想帳
                    Result07.Text = investmentAmount.ToString(); // 投資帳
                    Result08.Text = backupAmount.ToString(); // 備用帳
                }
                else
                {
                    // 顯示錯誤訊息，例如，百分比為0時無法進行分配
                    MessageBox.Show("夢想、投資、備用帳戶的總百分比不能為零。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // 顯示錯誤訊息，無法從輸入中獲取有效值
                MessageBox.Show("請檢查輸入的值是否有效。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryGetIntValues(out int m_income, out int m_pocketmoney, out int m_monthly, out int m_annual, out int m_home, out int m_public, out int m_dream, out int m_investment, out int m_backup)
        {
            m_pocketmoney = 0;
            m_monthly = 0;
            m_annual = 0;
            m_home = 0;
            m_public = 0;
            m_dream = 0;
            m_investment = 0;
            m_backup = 0;
            return int.TryParse(tbIncome.Text, out m_income) &&
                   int.TryParse(tbPocketmoney.Text, out m_pocketmoney) &&
                   int.TryParse(tbMonthly.Text, out m_monthly) &&
                   int.TryParse(tbAnnual.Text, out m_annual) &&
                   int.TryParse(tbHome.Text, out m_home) &&
                   int.TryParse(tbPublic.Text, out m_public) &&
                   int.TryParse(tbDream.Text, out m_dream) &&
                   int.TryParse(tbInvestment.Text, out m_investment) &&
                   int.TryParse(tbBackup.Text, out m_backup);
        }

        private void btnNumber_Click(object sender, EventArgs e)
        {
            if ((tbResult.Text == "0") || (isOperationPerformed))
                tbResult.Clear();

            isOperationPerformed = false;
            Button button = (Button)sender;
            if (button.Text == ".")
            {
                if (!tbResult.Text.Contains("."))
                    tbResult.Text = tbResult.Text + button.Text;
            }
            else
                tbResult.Text = tbResult.Text + button.Text;
        }

        private void operator_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (resultValue != 0)
            {
                btnEqual.PerformClick();
                operationPerformed = button.Text;
                tbResult.Text = tbResult.Text + button.Text;
                isOperationPerformed = true;
            }
            else
            {
                operationPerformed = button.Text;
                resultValue = Double.Parse(tbResult.Text);
                isOperationPerformed = true;
                tbResult.Text = tbResult.Text + button.Text;
            }
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            switch (operationPerformed)
            {
                case "+":
                    tbResult.Text = (resultValue + Double.Parse(tbResult.Text)).ToString();
                    break;
                case "-":
                    tbResult.Text = (resultValue - Double.Parse(tbResult.Text)).ToString();
                    break;
                case "*":
                    tbResult.Text = (resultValue * Double.Parse(tbResult.Text)).ToString();
                    break;
                case "/":
                    if (tbResult.Text != "0")
                        tbResult.Text = (resultValue / Double.Parse(tbResult.Text)).ToString();
                    else
                        tbResult.Text = "Cannot divide by zero";
                    break;
            }
            resultValue = Double.Parse(tbResult.Text);
            operationPerformed = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbResult.Text = "0";
            resultValue = 0;
        }
    }
}