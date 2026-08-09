using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace TestJob2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        #region События

        private void Form1_Load(object sender, EventArgs e)
        {
            var currentPath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            var folderStructurePath = Path.Combine(currentPath.FullName, "TesFolders", "Документы", "Договоры", "Общие");
            if (!Directory.Exists(folderStructurePath))
            {
                MessageBox.Show("Тестовая труктура папок с документаим не найдена. Выбереите корневой каталог структуры папок для поиска по кнопке \"...\" на форме.");
            }
            else
            {
                rootPathTextBox.Text = folderStructurePath;
                targetPathTextBox.Text = Path.Combine(currentPath.FullName, "TesFolders", "Документы", "Договоры", "На печать");
            }
        }

        /// <summary>
        /// Выбрать корневой каталог.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            var path = rootPathTextBox.Text;
            if (string.IsNullOrEmpty(path))
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            else
                folderDialog.SelectedPath = path;

            folderDialog.Description = "Выберети корневую папку для работы с документами";

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                rootPathTextBox.Text = folderDialog.SelectedPath;
            }

        }

        /// <summary>
        /// Выбрать каталог поиска.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            folderDialog.Description = "Выберите папку с документами";
            var root = rootPathTextBox.Text;
            var relativePath = folderPathTextBox.Text;
            if (string.IsNullOrEmpty(relativePath))
                folderDialog.SelectedPath = root;
            else
                folderDialog.SelectedPath = Path.Combine(root, relativePath);

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPathTextBox.Text = CheckFolder(folderDialog.SelectedPath) ? folderDialog.SelectedPath.Substring(root.Length) : relativePath;
            }
        }

        /// <summary>
        /// Выбрать документы для обработки.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            var folderPath = folderPathTextBox.Text.Replace('/', Path.DirectorySeparatorChar);
            folderPath = folderPath[0] == Path.DirectorySeparatorChar ? folderPath.Substring(1) : folderPath;
            var fullPath = Path.Combine(rootPathTextBox.Text, folderPath);
            if (!CheckFolder(fullPath))
                return;

            var files = Directory.GetFiles(fullPath).ToList();
            if (files.Any())
            {
                var templateNames = new List<string>();
                var regExp = new Regex(@"[a-zA-Zа-яА-Я\s]+");

                foreach (var file in files)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var template = regExp.Match(name).Value.Trim();
                    if (!templateNames.Any(x => x.ToLower().Equals(template.ToLower())))
                        templateNames.Add(template);
                }

                Form2 dialog = new Form2();
                var listBox = dialog.DocumentsListBox;
                foreach (var item in templateNames)
                {
                    listBox.Items.Add(item);
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var targetFolder = targetPathTextBox.Text;
                    var targetFolderFiles = Directory.GetFiles(targetFolder).ToList();
                    if (targetFolderFiles.Any())
                    {
                        DialogResult result = MessageBox.Show("В целевой папке уже есть документы. Удалить их? Не удаленные файлы могут быть перезаписаны.", 
                                                              "Подтверждение", 
                                                              MessageBoxButtons.YesNo, 
                                                              MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            foreach (var targetFile in targetFolderFiles)
                                File.Delete(targetFile);
                        }
                    }

                    var selecteTypes = new List<string>();
                    for (int i = 0; i < listBox.CheckedItems.Count; i++)
                        selecteTypes.Add(listBox.CheckedItems[i].ToString());


                    foreach (var type in selecteTypes)
                    {
                        var i = 0;
                        foreach (var filePath in files)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            var extension = Path.GetExtension(filePath);

                            if (fileName.ToLower().Contains(type.ToLower()))
                            {
                                File.Copy(filePath, Path.Combine(targetFolder, type + (i == 0? string.Empty : $" ({i})") + extension));
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Документы не найдены");
            }
        }

        /// <summary>
        /// Выбрать конечный каталог.
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            folderDialog.Description = "Выберите целевую папку для копирования";
            var root = rootPathTextBox.Text;
            var relativePath = targetPathTextBox.Text;
            if (string.IsNullOrEmpty(relativePath))
                folderDialog.SelectedPath = root;
            else
                folderDialog.SelectedPath = Path.Combine(root, relativePath);

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPathTextBox.Text = CheckFolder(folderDialog.SelectedPath) ? folderDialog.SelectedPath.Substring(root.Length) : relativePath;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверить папку.
        /// </summary>
        /// <param name="path">Путь к папке</param>
        /// <returns>True - если папка существуе и содержит документы, иначе False.</returns>
        public bool CheckFolder(string path)
        {
            var result = false;
            try
            {
                if (!Directory.Exists(path))
                {
                    MessageBox.Show($"Папка {path} не найдена. Проверьте введенный путь к папке или выберете ее с помощью кнопки \"...\" на форме");
                    return result;
                }
                if (!Directory.GetFiles(path).ToList().Any())
                {
                    MessageBox.Show($"Документы в папке {path} не найдены. Проверьте введенный путь к папке или выберете ее с помощью кнопки \"...\" на форме");
                    return result;
                }

                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return result;
        }

        #endregion

    }
}
