using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services.Conversation;
using WindowsFormsApp1.Services.Exporter;

namespace WindowsFormsApp1
{
    public class AppChatsExtractor : Form
    {
        // Services
        private ConversationService conversationService;
        private ExporterService exporterService;

        // Models
        private List<ConversationModel> conversationModels;

        //Components
        private Button selectFolderButton;
        private Button generateReportButton;
        private Label conversationsLabel;
        private Label selectedConversationDisplayLabel;
        private Label selectedFolderLabel;
        private Label identificationNumberLabel;
        private Label ownerNameLabel;
        private Label importationReportLabel;
        private TextBox selectedConversationDisplayTextBox;
        private TextBox ownerNameInput;
        private TextBox identificationNumberInput;
        private TextBox importationReportTextBox;
        private ListBox conversationsListBox;

        // Dialogs
        private FolderBrowserDialog folderBrowserDialog1;

        public AppChatsExtractor()
        {
            conversationService = new ConversationService();
            exporterService = new ExporterService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppChatsExtractor));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.conversationsListBox = new System.Windows.Forms.ListBox();
            this.generateReportButton = new System.Windows.Forms.Button();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.selectedConversationDisplayTextBox = new System.Windows.Forms.TextBox();
            this.conversationsLabel = new System.Windows.Forms.Label();
            this.selectedConversationDisplayLabel = new System.Windows.Forms.Label();
            this.selectedFolderLabel = new System.Windows.Forms.Label();
            this.ownerNameInput = new System.Windows.Forms.TextBox();
            this.identificationNumberInput = new System.Windows.Forms.TextBox();
            this.ownerNameLabel = new System.Windows.Forms.Label();
            this.identificationNumberLabel = new System.Windows.Forms.Label();
            this.importationReportTextBox = new System.Windows.Forms.TextBox();
            this.importationReportLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Selecione o diretório com as pastas de conversas.";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // conversationsListBox
            // 
            this.conversationsListBox.FormattingEnabled = true;
            this.conversationsListBox.Location = new System.Drawing.Point(25, 73);
            this.conversationsListBox.Name = "conversationsListBox";
            this.conversationsListBox.Size = new System.Drawing.Size(344, 160);
            this.conversationsListBox.TabIndex = 0;
            this.conversationsListBox.Click += new System.EventHandler(this.conversationSelected);
            // 
            // generateReportButton
            // 
            this.generateReportButton.Enabled = false;
            this.generateReportButton.Location = new System.Drawing.Point(205, 528);
            this.generateReportButton.Name = "generateReportButton";
            this.generateReportButton.Size = new System.Drawing.Size(164, 23);
            this.generateReportButton.TabIndex = 1;
            this.generateReportButton.Text = "Gerar Relatório de Extração";
            this.generateReportButton.UseVisualStyleBackColor = true;
            this.generateReportButton.Click += new System.EventHandler(this.generateReportButtonClick);
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(25, 9);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(207, 23);
            this.selectFolderButton.TabIndex = 2;
            this.selectFolderButton.Text = "Selecione o diretório das conversas";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButtonClick);
            // 
            // selectedConversationDisplayTextBox
            // 
            this.selectedConversationDisplayTextBox.Location = new System.Drawing.Point(390, 73);
            this.selectedConversationDisplayTextBox.Multiline = true;
            this.selectedConversationDisplayTextBox.Name = "selectedConversationDisplayTextBox";
            this.selectedConversationDisplayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.selectedConversationDisplayTextBox.Size = new System.Drawing.Size(485, 478);
            this.selectedConversationDisplayTextBox.TabIndex = 3;
            // 
            // conversationsLabel
            // 
            this.conversationsLabel.AutoSize = true;
            this.conversationsLabel.Location = new System.Drawing.Point(25, 54);
            this.conversationsLabel.Name = "conversationsLabel";
            this.conversationsLabel.Size = new System.Drawing.Size(112, 13);
            this.conversationsLabel.TabIndex = 4;
            this.conversationsLabel.Text = "Conversas Importadas";
            // 
            // selectedConversationDisplayLabel
            // 
            this.selectedConversationDisplayLabel.AutoSize = true;
            this.selectedConversationDisplayLabel.Location = new System.Drawing.Point(390, 53);
            this.selectedConversationDisplayLabel.Name = "selectedConversationDisplayLabel";
            this.selectedConversationDisplayLabel.Size = new System.Drawing.Size(151, 13);
            this.selectedConversationDisplayLabel.TabIndex = 5;
            this.selectedConversationDisplayLabel.Text = "Texto da Conversa seleionada";
            // 
            // selectedFolderLabel
            // 
            this.selectedFolderLabel.Location = new System.Drawing.Point(252, 9);
            this.selectedFolderLabel.Name = "selectedFolderLabel";
            this.selectedFolderLabel.Size = new System.Drawing.Size(623, 23);
            this.selectedFolderLabel.TabIndex = 13;
            // 
            // ownerNameInput
            // 
            this.ownerNameInput.Location = new System.Drawing.Point(116, 456);
            this.ownerNameInput.Name = "ownerNameInput";
            this.ownerNameInput.Size = new System.Drawing.Size(253, 20);
            this.ownerNameInput.TabIndex = 7;
            this.ownerNameInput.LostFocus += new System.EventHandler(this.checkButtonActivation);
            // 
            // identificationNumberInput
            // 
            this.identificationNumberInput.Location = new System.Drawing.Point(116, 489);
            this.identificationNumberInput.Name = "identificationNumberInput";
            this.identificationNumberInput.Size = new System.Drawing.Size(253, 20);
            this.identificationNumberInput.TabIndex = 8;
            this.identificationNumberInput.TextChanged += new System.EventHandler(this.checkButtonActivation);
            // 
            // ownerNameLabel
            // 
            this.ownerNameLabel.AutoSize = true;
            this.ownerNameLabel.Location = new System.Drawing.Point(25, 461);
            this.ownerNameLabel.Name = "ownerNameLabel";
            this.ownerNameLabel.Size = new System.Drawing.Size(49, 13);
            this.ownerNameLabel.TabIndex = 9;
            this.ownerNameLabel.Text = "Perito(a):";
            // 
            // identificationNumberLabel
            // 
            this.identificationNumberLabel.AutoSize = true;
            this.identificationNumberLabel.Location = new System.Drawing.Point(25, 492);
            this.identificationNumberLabel.Name = "identificationNumberLabel";
            this.identificationNumberLabel.Size = new System.Drawing.Size(70, 13);
            this.identificationNumberLabel.TabIndex = 10;
            this.identificationNumberLabel.Text = "Nº Protocolo:";
            // 
            // importationReportTextBox
            // 
            this.importationReportTextBox.Location = new System.Drawing.Point(25, 265);
            this.importationReportTextBox.Multiline = true;
            this.importationReportTextBox.Name = "importationReportTextBox";
            this.importationReportTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.importationReportTextBox.Size = new System.Drawing.Size(344, 168);
            this.importationReportTextBox.TabIndex = 11;
            // 
            // importationReportLabel
            // 
            this.importationReportLabel.AutoSize = true;
            this.importationReportLabel.Location = new System.Drawing.Point(28, 246);
            this.importationReportLabel.Name = "importationReportLabel";
            this.importationReportLabel.Size = new System.Drawing.Size(117, 13);
            this.importationReportLabel.TabIndex = 12;
            this.importationReportLabel.Text = "Resumo da Importação";
            // 
            // AppChatsExtractor
            // 
            this.ClientSize = new System.Drawing.Size(900, 576);
            this.Controls.Add(this.importationReportLabel);
            this.Controls.Add(this.importationReportTextBox);
            this.Controls.Add(this.identificationNumberLabel);
            this.Controls.Add(this.ownerNameLabel);
            this.Controls.Add(this.identificationNumberInput);
            this.Controls.Add(this.ownerNameInput);
            this.Controls.Add(this.selectedFolderLabel);
            this.Controls.Add(this.selectedConversationDisplayLabel);
            this.Controls.Add(this.conversationsLabel);
            this.Controls.Add(this.selectedConversationDisplayTextBox);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.generateReportButton);
            this.Controls.Add(this.conversationsListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AppChatsExtractor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transcritor de Conversas de Aplicativo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void conversationSelected(object sender, EventArgs e)
        {
            selectedConversationDisplayTextBox.Clear();

            var selectedConversation = conversationsListBox.SelectedItem;

            var conversation = conversationModels.FirstOrDefault(c => c.ConversationTitle == selectedConversation);

            if (conversation != null)
            {
                var sb = new StringBuilder();

                foreach (var message in conversation.Messages)
                {
                    sb.AppendLine($"{message.Timestamp} - {message.Author} - {message.Text}");
                }

                selectedConversationDisplayTextBox.Text = sb.ToString();
            }
        }

        private void checkButtonActivation(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(identificationNumberInput.Text) && !string.IsNullOrEmpty(ownerNameInput.Text))
                generateReportButton.Enabled = true;
        }

        private void selectFolderButtonClick(object sender, EventArgs e)
        {
            generateReportButton.Enabled = false;
            conversationsListBox.Items.Clear();
            importationReportTextBox.Clear();
            conversationModels = new List<ConversationModel>();
            selectedFolderLabel.Text = "";

            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                selectedFolderLabel.Text = folderBrowserDialog1.SelectedPath;

                var allFolders = Directory.GetDirectories(folderBrowserDialog1.SelectedPath);

                var reportSb = new StringBuilder();

                foreach (var folderPath in allFolders)
                {
                    var conversationModel = conversationService.GetConversation(folderPath);
                    reportSb.AppendLine(conversationModel.ImportReport);
                    conversationsListBox.Items.Add(conversationModel.ConversationTitle);
                    conversationModels.Add(conversationModel);
                }

                importationReportTextBox.Text = reportSb.ToString();
            }
        }

        private void generateReportButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(identificationNumberInput.Text) || string.IsNullOrEmpty(ownerNameInput.Text))
            {
                MessageBox.Show(this, "O nome do perito(a) e o número de identificação deven ser preenchidos!", "Atenção");
                return;
            }

            var htmlSB = new StringBuilder();

            foreach (var model in conversationModels)
            {
                htmlSB.Append(conversationService.GetChatHtml(model));
            }

            // TODO: resolve pdf generation issue (component allows only five pages)
            //var pdfFileBytes = exporterService.ExportToPdfFileBytes(textBox2.Text, textBox3.Text, htmlSB.ToString());
            //SaveReportAsPdf(pdfFileBytes);

            var htmlString = exporterService.ExportToHTMLFile(ownerNameInput.Text, identificationNumberInput.Text, htmlSB.ToString());
            SaveReportAsHtml(htmlString);
        }

        private void SaveReportAsPdf(byte[] fileBytes)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Arquivos Pdf|*.pdf";
            saveFileDialog1.Title = "Salvar Relatório";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllBytes(saveFileDialog1.FileName, fileBytes);
            }
        }

        private void SaveReportAsHtml(string htmlString)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Arquivos Html|*.html";
            saveFileDialog1.Title = "Salvar Relatório";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, htmlString);
            }
        }
    }
}
