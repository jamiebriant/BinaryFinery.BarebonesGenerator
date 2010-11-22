// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BinaryFinery.BareBonesGenerator.AddIn.Generators;
using Reflector;
using Reflector.CodeModel;
using Spackle;

namespace BinaryFinery.BareBonesGenerator.AddIn.UI
{
    public class FileGeneratorControl : UserControl
    {
        private const string FolderDialogDescription = "Select the folder that will contain the code files.";

        private readonly IAssemblyBrowser assemblyBrowser;
        private readonly IServiceProvider serviceProvider;
        private Button browseDirectoriesButton;
        private ManualResetEvent cancel;
        private Button cancelGenerationButton;
        private ManualResetEvent complete;
        private CheckBox createSubDirectories;
        private CheckBox createVisualStudioProjectFile;
        private ProgressBar fileGenerationProgress;
        private TextBox fileGenerationStatusText;
        private Button generateFilesButton;
        private Label outputDirectoryLabel;
        private TextBox outputDirectoryText;
        private Label targetLabel;
        private int typeCount;
        private int typesGenerated;

        public FileGeneratorControl()
        {
            InitializeComponent();
            cancelGenerationButton.Enabled = false;
        }

        public FileGeneratorControl(IServiceProvider serviceProvider)
            : this()
        {
            this.serviceProvider = serviceProvider;
            assemblyBrowser = (IAssemblyBrowser) this.serviceProvider.GetService(typeof(IAssemblyBrowser));
            assemblyBrowser.ActiveItemChanged += OnAssemblyBrowserActiveItemChanged;
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outputDirectoryLabel = new System.Windows.Forms.Label();
            this.outputDirectoryText = new System.Windows.Forms.TextBox();
            this.browseDirectoriesButton = new System.Windows.Forms.Button();
            this.fileGenerationProgress = new System.Windows.Forms.ProgressBar();
            this.generateFilesButton = new System.Windows.Forms.Button();
            this.cancelGenerationButton = new System.Windows.Forms.Button();
            this.targetLabel = new System.Windows.Forms.Label();
            this.createSubDirectories = new System.Windows.Forms.CheckBox();
            this.createVisualStudioProjectFile = new System.Windows.Forms.CheckBox();
            this.fileGenerationStatusText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // outputDirectoryLabel
            // 
            this.outputDirectoryLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputDirectoryLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
                                                                     System.Drawing.GraphicsUnit.Point, ((0)));
            this.outputDirectoryLabel.Location = new System.Drawing.Point(8, 12);
            this.outputDirectoryLabel.Name = "outputDirectoryLabel";
            this.outputDirectoryLabel.Size = new System.Drawing.Size(88, 16);
            this.outputDirectoryLabel.TabIndex = 0;
            this.outputDirectoryLabel.Text = "Output Directory:";
            this.outputDirectoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // outputDirectoryText
            // 
            this.outputDirectoryText.Anchor = ((((System.Windows.Forms.AnchorStyles.Top |
                                                  System.Windows.Forms.AnchorStyles.Left)
                                                 | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirectoryText.Location = new System.Drawing.Point(96, 8);
            this.outputDirectoryText.Name = "outputDirectoryText";
            this.outputDirectoryText.Size = new System.Drawing.Size(464, 21);
            this.outputDirectoryText.TabIndex = 1;
            this.outputDirectoryText.Text = "c:\\clients\\gen3";
            // 
            // browseDirectoriesButton
            // 
            this.browseDirectoriesButton.Anchor = (((System.Windows.Forms.AnchorStyles.Top |
                                                     System.Windows.Forms.AnchorStyles.Right)));
            this.browseDirectoriesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseDirectoriesButton.Location = new System.Drawing.Point(568, 8);
            this.browseDirectoriesButton.Name = "browseDirectoriesButton";
            this.browseDirectoriesButton.Size = new System.Drawing.Size(75, 23);
            this.browseDirectoriesButton.TabIndex = 2;
            this.browseDirectoriesButton.Text = "Browse...";
            this.browseDirectoriesButton.Click += this.OnBrowseDirectoriesButtonClick;
            // 
            // fileGenerationProgress
            // 
            this.fileGenerationProgress.Anchor = ((((System.Windows.Forms.AnchorStyles.Bottom |
                                                     System.Windows.Forms.AnchorStyles.Left)
                                                    | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGenerationProgress.Location = new System.Drawing.Point(8, 220);
            this.fileGenerationProgress.Name = "fileGenerationProgress";
            this.fileGenerationProgress.Size = new System.Drawing.Size(632, 23);
            this.fileGenerationProgress.TabIndex = 9;
            // 
            // generateFilesButton
            // 
            this.generateFilesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.generateFilesButton.Location = new System.Drawing.Point(8, 62);
            this.generateFilesButton.Name = "generateFilesButton";
            this.generateFilesButton.Size = new System.Drawing.Size(88, 23);
            this.generateFilesButton.TabIndex = 5;
            this.generateFilesButton.Text = "Generate Files";
            this.generateFilesButton.Click += this.OnGenerateFilesButtonClick;
            // 
            // cancelGenerationButton
            // 
            this.cancelGenerationButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelGenerationButton.Location = new System.Drawing.Point(104, 62);
            this.cancelGenerationButton.Name = "cancelGenerationButton";
            this.cancelGenerationButton.Size = new System.Drawing.Size(88, 23);
            this.cancelGenerationButton.TabIndex = 6;
            this.cancelGenerationButton.Text = "Cancel";
            this.cancelGenerationButton.Click += this.OnCancelGenerationButtonClick;
            // 
            // targetLabel
            // 
            this.targetLabel.Anchor = ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.targetLabel.Location = new System.Drawing.Point(9, 93);
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Size = new System.Drawing.Size(632, 16);
            this.targetLabel.TabIndex = 8;
            // 
            // createSubDirectories
            // 
            this.createSubDirectories.AutoSize = true;
            this.createSubDirectories.Location = new System.Drawing.Point(8, 39);
            this.createSubDirectories.Name = "createSubDirectories";
            this.createSubDirectories.Size = new System.Drawing.Size(130, 17);
            this.createSubDirectories.TabIndex = 3;
            this.createSubDirectories.Text = "Create Subdirectories";
            this.createSubDirectories.UseVisualStyleBackColor = true;
            // 
            // createVisualStudioProjectFile
            // 
            this.createVisualStudioProjectFile.AutoSize = true;
            this.createVisualStudioProjectFile.Location = new System.Drawing.Point(143, 39);
            this.createVisualStudioProjectFile.Name = "createVisualStudioProjectFile";
            this.createVisualStudioProjectFile.Size = new System.Drawing.Size(178, 17);
            this.createVisualStudioProjectFile.TabIndex = 4;
            this.createVisualStudioProjectFile.Text = "Create Visual Studio Project File";
            this.createVisualStudioProjectFile.UseVisualStyleBackColor = true;
            // 
            // fileGenerationStatusText
            // 
            this.fileGenerationStatusText.Anchor = (((((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Bottom)
                                                       | System.Windows.Forms.AnchorStyles.Left)
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGenerationStatusText.Location = new System.Drawing.Point(8, 123);
            this.fileGenerationStatusText.MaxLength = 0;
            this.fileGenerationStatusText.Multiline = true;
            this.fileGenerationStatusText.Name = "fileGenerationStatusText";
            this.fileGenerationStatusText.ReadOnly = true;
            this.fileGenerationStatusText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fileGenerationStatusText.Size = new System.Drawing.Size(632, 86);
            this.fileGenerationStatusText.TabIndex = 10;
            // 
            // FileGeneratorControl
            // 
            this.Controls.Add(this.fileGenerationStatusText);
            this.Controls.Add(this.createVisualStudioProjectFile);
            this.Controls.Add(this.createSubDirectories);
            this.Controls.Add(this.targetLabel);
            this.Controls.Add(this.cancelGenerationButton);
            this.Controls.Add(this.generateFilesButton);
            this.Controls.Add(this.fileGenerationProgress);
            this.Controls.Add(this.browseDirectoriesButton);
            this.Controls.Add(this.outputDirectoryText);
            this.Controls.Add(this.outputDirectoryLabel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
                                                System.Drawing.GraphicsUnit.Point, ((0)));
            this.Name = "FileGeneratorControl";
            this.Size = new System.Drawing.Size(648, 252);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void CancelFileGeneration()
        {
            if (cancel != null && complete != null)
            {
                cancel.Set();

                var finished = false;

                do
                {
                    finished = complete.WaitOne(FileGeneratorFactory.EventWaitTime, false);
                    Application.DoEvents();
                } while (!finished);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelFileGeneration();
                cancel.Close();
                complete.Close();
            }

            base.Dispose(disposing);
        }

        private static object Resolve(object activeItem)
        {
            IAssembly assemblyToResolve = null;
            bool allAreResolved = true;

            if (activeItem is IAssembly)
            {
                assemblyToResolve = (IAssembly) activeItem;
            }
            else if (activeItem is IModule)
            {
                assemblyToResolve = ((IModule) activeItem).Assembly;
            }
            else if (activeItem is INamespace)
            {
                var namespaceActiveItem = (INamespace) activeItem;

                if (namespaceActiveItem.Types != null && namespaceActiveItem.Types.Count > 0)
                {
                    assemblyToResolve = ((IModule) namespaceActiveItem.Types[0].Owner).Assembly;
                }
            }
            else if (activeItem is ITypeDeclaration)
            {
                var typeActiveItem = (ITypeDeclaration) activeItem;
                assemblyToResolve = ((IModule) typeActiveItem.Owner).Assembly;
            }

            if (assemblyToResolve != null)
            {
                foreach (IModule module in assemblyToResolve.Modules)
                {
                    foreach (IAssemblyReference assemblyReferenceName in module.AssemblyReferences)
                    {
                        var resolvedAssembly = assemblyReferenceName.Resolve();

                        if (resolvedAssembly == null)
                        {
                            allAreResolved = false;
                            break;
                        }
                    }
                }
            }

            if (allAreResolved == false)
            {
                assemblyToResolve = null;
            }

            return assemblyToResolve;
        }

        private void FileGenerated(FileGeneratedEventArgs fileInfo)
        {
            typesGenerated++;
            fileGenerationProgress.Increment(1);
            fileGenerationProgress.Refresh();

            fileGenerationStatusText.Text = fileInfo.FileName + " is generated.";
            fileGenerationStatusText.Refresh();
        }

        private void FileGenerationComplete()
        {
            cancelGenerationButton.Enabled = false;

            var results = new StringBuilder();

            results.Append("Total number of types: ").Append(typeCount).Append(Environment.NewLine)
                .Append("Total number of generated types: ").Append(typesGenerated);

            fileGenerationStatusText.Text = results.ToString();
            fileGenerationStatusText.SelectionStart = 0;
            fileGenerationStatusText.SelectionLength = 0;
            UpdateUIState();
        }

        private void GenerateFiles(object data)
        {
            cancel = new ManualResetEvent(false);
            complete = new ManualResetEvent(false);
            typesGenerated = 0;

            try
            {
                var state = data as GenerateFilesState;
                var languageManager = (ILanguageManager) serviceProvider.GetService(typeof(ILanguageManager));
                var language = languageManager.ActiveLanguage;
                var visitorManager = (ITranslatorManager) serviceProvider.GetService(typeof(ITranslatorManager));
                var visitor = visitorManager.CreateDisassembler(null, null);

                var fileGenerator = FileGeneratorFactory.Create(
                    state.ActiveItem, state.Directory, visitor,
                    language, cancel, state.CreateSubdirectories,
                    state.CreateVisualStudioProjectFile);

                Invoke(new SetTargetInformationHandler(SetTargetInformation),
                       new[] {state.ActiveItem});

                typeCount = fileGenerator.TypeCount;

                if (fileGenerator != null)
                {
                    fileGenerator.FileCreated += OnFileGenerated;

                    Invoke(new SetupProgressBarHandler(SetupProgressBar),
                           new object[] {fileGenerator.TypeCount});

                    try
                    {
                        fileGenerator.Generate();
                    }
                    finally
                    {
                        fileGenerator.FileCreated -= OnFileGenerated;
                    }
                }
            }
            finally
            {
                complete.Set();
                Invoke(new FileGenerationCompleteHandler(FileGenerationComplete));
            }
        }

        private void OnAssemblyBrowserActiveItemChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void OnBrowseDirectoriesButtonClick(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = FolderDialogDescription;
                folderDialog.ShowNewFolderButton = true;

                if (Directory.Exists(outputDirectoryText.Text))
                {
                    folderDialog.SelectedPath = outputDirectoryText.Text;
                }
                else
                {
                    folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                }

                var folderResult = folderDialog.ShowDialog(ParentForm);

                if (folderResult == DialogResult.OK)
                {
                    outputDirectoryText.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void OnCancelGenerationButtonClick(object sender, EventArgs e)
        {
            using (var switcher = new ScopeSwitcher<Control, Cursor>(Parent, Cursors.WaitCursor))
            {
                CancelFileGeneration();
                FileGenerationComplete();
            }
        }

        private void OnFileGenerated(object sender, FileGeneratedEventArgs fileInfo)
        {
            Invoke(new FileGeneratedHandler(FileGenerated), new object[] {fileInfo});
        }

        private void OnGenerateFilesButtonClick(object sender, EventArgs e)
        {
            if (outputDirectoryText.Text != null && outputDirectoryText.Text.Trim().Length > 0)
            {
                if (Directory.Exists(outputDirectoryText.Text) == false)
                {
                    Directory.CreateDirectory(outputDirectoryText.Text);
                }

                using (var switcher = new ScopeSwitcher<Control, Cursor>(Parent, Cursors.WaitCursor))
                {
                    var assemblyBrowser = (IAssemblyBrowser) serviceProvider.GetService(typeof(IAssemblyBrowser));

                    if (assemblyBrowser.ActiveItem != null)
                    {
                        object resolvedObject = Resolve(assemblyBrowser.ActiveItem);

                        if (resolvedObject != null)
                        {
                            if (ThreadPool.QueueUserWorkItem(GenerateFiles,
                                                             new GenerateFilesState
                                                                 {
                                                                     ActiveItem = assemblyBrowser.ActiveItem,
                                                                     CreateSubdirectories = createSubDirectories.Checked,
                                                                     CreateVisualStudioProjectFile =
                                                                         createVisualStudioProjectFile.Checked,
                                                                     Directory = outputDirectoryText.Text
                                                                 }))
                            {
                                generateFilesButton.Enabled = false;
                                cancelGenerationButton.Enabled = true;
                            }
                        }
                        else
                        {
                            fileGenerationStatusText.Text = string.Format(
                                CultureInfo.CurrentCulture, "Could not resolve the active item type {0}.",
                                assemblyBrowser.ActiveItem.GetType().FullName);
                            fileGenerationStatusText.SelectionStart = 0;
                            fileGenerationStatusText.SelectionLength = 0;
                        }
                    }
                }
            }
        }

        private void SetTargetInformation(object activeItem)
        {
            if (activeItem is IAssembly)
            {
                targetLabel.Text = "Assembly: " + ((IAssembly) activeItem).Name;
            }
            else if (activeItem is IModule)
            {
                targetLabel.Text = "Module: " + ((IModule) activeItem).Name;
            }
            else if (activeItem is INamespace)
            {
                targetLabel.Text = "Namespace: " + ((INamespace) activeItem).Name;
            }
            else if (activeItem is ITypeDeclaration)
            {
                var type = (ITypeDeclaration) activeItem;
                targetLabel.Text = string.Format(CultureInfo.CurrentCulture,
                                                 "Type: {0}.{1}", type.Namespace, type.Name);
            }
        }

        private void SetupProgressBar(int typeCount)
        {
            fileGenerationProgress.Minimum = 0;
            fileGenerationProgress.Maximum = typeCount;
            fileGenerationProgress.Value = 0;
        }

        private void UpdateUIState()
        {
            if (complete != null &&
                complete.WaitOne(FileGeneratorFactory.EventWaitTime, false) == false)
            {
                generateFilesButton.Enabled = false;
                createVisualStudioProjectFile.Enabled = false;
            }
            else
            {
                if (Visible)
                {
                    generateFilesButton.Enabled =
                        (assemblyBrowser.ActiveItem is IModule ||
                         assemblyBrowser.ActiveItem is ITypeDeclaration ||
                         assemblyBrowser.ActiveItem is IAssembly ||
                         assemblyBrowser.ActiveItem is INamespace);
                    createVisualStudioProjectFile.Enabled = generateFilesButton.Enabled;
                }
            }
        }

        #region Nested type: FileGeneratedHandler

        private delegate void FileGeneratedHandler(FileGeneratedEventArgs fileInfo);

        #endregion

        #region Nested type: FileGenerationCompleteHandler

        private delegate void FileGenerationCompleteHandler();

        #endregion

        #region Nested type: GenerateFilesState

        private sealed class GenerateFilesState
        {
            public object ActiveItem { get; set; }

            public bool CreateSubdirectories { get; set; }

            public bool CreateVisualStudioProjectFile { get; set; }

            public string Directory { get; set; }
        }

        #endregion

        #region Nested type: SetTargetInformationHandler

        private delegate void SetTargetInformationHandler(object activeItem);

        #endregion

        #region Nested type: SetupProgressBarHandler

        private delegate void SetupProgressBarHandler(int typeCount);

        #endregion
    }
}