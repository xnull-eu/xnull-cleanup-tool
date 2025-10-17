using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Security.Principal;

namespace XNullCleanup
{
    public partial class MainForm : Form
    {
        // Constants for custom window styling
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        // For custom window movement
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        // Dictionary to store cleanup options and their descriptions
        private Dictionary<string, CleanupOption> cleanupOptions = new Dictionary<string, CleanupOption>();
        
        // Currently selected option
        private string? selectedOption = null;

        // Material 3 Colors
        private readonly Color primaryColor = Color.FromArgb(103, 80, 164);      // Deep purple
        private readonly Color onPrimaryColor = Color.FromArgb(255, 255, 255);   // White
        private readonly Color surfaceColor = Color.FromArgb(255, 251, 254);     // Light background
        private readonly Color darkSurfaceColor = Color.FromArgb(28, 27, 31);    // Dark background
        private readonly Color surfaceVariantColor = Color.FromArgb(231, 224, 236); // Light surface variant
        private readonly Color darkSurfaceVariantColor = Color.FromArgb(73, 69, 79); // Dark surface variant
        private readonly Color onSurfaceColor = Color.FromArgb(28, 27, 31);      // Dark text on light bg
        private readonly Color onDarkSurfaceColor = Color.FromArgb(230, 225, 229); // Light text on dark bg
        private readonly Color errorColor = Color.FromArgb(186, 26, 26);         // Error color
        private readonly Color secondaryColor = Color.FromArgb(98, 91, 113);     // Secondary color

        // Custom scrollbar
        private CustomScrollBar customScrollBar = null!;
        
        // Material Select All checkbox reference
        private MaterialCheckBox? materialSelectAllCheckBox = null;
        
        // Flag to prevent infinite loop when updating checkboxes
        private bool isUpdatingSelectAll = false;
        
        // Timer for automatic size refresh
        private System.Windows.Forms.Timer? sizeRefreshTimer;

        public MainForm()
        {
            InitializeComponent();
            InitializeCleanupOptions();
            
            // Configure RichTextBox
            descriptionTextBox.ReadOnly = true;
            descriptionTextBox.BackColor = IsDarkMode() ? darkSurfaceVariantColor : surfaceVariantColor;
            descriptionTextBox.BorderStyle = BorderStyle.None;
            descriptionTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            
            ApplyMaterial3Theme();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set the form title
            this.Text = "XNull Cleanup Tool";
            titleLabel.Text = "XNull Cleanup Tool";

            // Set the form icon
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "cleanup.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading icon: {ex.Message}");
            }

            // Populate the cleanup options panel
            PopulateCleanupOptions();
            
            // Setup the custom scrollbar after populating
            SetupCustomScrollBar();
            
            // Initialize automatic size refresh timer
            InitializeSizeRefreshTimer();
        }

        private void InitializeSizeRefreshTimer()
        {
            sizeRefreshTimer = new System.Windows.Forms.Timer();
            sizeRefreshTimer.Interval = 5000; // 5 seconds
            sizeRefreshTimer.Tick += SizeRefreshTimer_Tick;
            sizeRefreshTimer.Start();
        }

        private void SizeRefreshTimer_Tick(object? sender, EventArgs e)
        {
            RefreshSizeDisplays();
        }

        private void RefreshSizeDisplays()
        {
            // Update size displays for all cleanup options
            foreach (Control control in cleanupOptionsPanel.Controls)
            {
                if (control is Panel panel)
                {
                    Label? label = panel.Controls.OfType<Label>().FirstOrDefault();
                    if (label != null && panel.Tag is string optionKey)
                    {
                        if (cleanupOptions.TryGetValue(optionKey, out CleanupOption? option) && option != null)
                        {
                            // Recreate label text with updated size
                            string labelText = optionKey;
                            
                            // Only show size for file/folder-based cleanups, not command-based ones
                            if (option.CustomCleanupFunction != null && string.IsNullOrEmpty(option.Path))
                            {
                                // Command-based cleanup (like DNS Cache) - don't show size
                                if (!option.Description.Contains("Recycle Bin"))
                                {
                                    labelText = optionKey; // No size display
                                }
                                else
                                {
                                    // Recycle Bin - show size
                                    string sizeText = option.GetFormattedSize();
                                    labelText = $"{optionKey} ({sizeText})";
                                }
                            }
                            else
                            {
                                // File/folder-based cleanup - show size
                                string sizeText = option.GetFormattedSize();
                                labelText = $"{optionKey} ({sizeText})";
                            }
                            
                            label.Text = labelText;
                        }
                    }
                }
            }
        }

        private void SetupCustomScrollBar()
        {
            // Remove the default scrollbar
            cleanupOptionsPanel.AutoScroll = false;
            cleanupOptionsPanel.HorizontalScroll.Maximum = 0;
            cleanupOptionsPanel.HorizontalScroll.Visible = false;
            cleanupOptionsPanel.VerticalScroll.Maximum = 0;
            cleanupOptionsPanel.VerticalScroll.Visible = false;

            // Create and add the custom scrollbar
            customScrollBar = new CustomScrollBar
            {
                Width = 12,
                BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor,
                ThumbColor = IsDarkMode() ? primaryColor : primaryColor,
                BorderColor = IsDarkMode() ? darkSurfaceColor : surfaceColor,
                AutoSize = false
            };

            // Position the scrollbar correctly
            customScrollBar.Location = new Point(cleanupOptionsPanel.Right - customScrollBar.Width, cleanupOptionsPanel.Top);
            customScrollBar.Height = cleanupOptionsPanel.Height;
            customScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            // Add the custom scrollbar to the panel's parent
            cleanupOptionsPanel.Parent.Controls.Add(customScrollBar);
            customScrollBar.BringToFront();

            // Handle the scroll events
            cleanupOptionsPanel.MouseWheel += CleanupOptionsPanel_MouseWheel;
            customScrollBar.Scroll += CustomScrollBar_Scroll;

            // Initial setup
            UpdateScrollBar();
        }

        private void CleanupOptionsPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            // Calculate new scroll position
            int newValue = customScrollBar.Value - (e.Delta / 120) * 20;
            customScrollBar.Value = Math.Max(customScrollBar.Minimum, Math.Min(customScrollBar.Maximum, newValue));
            
            // Update the panel's scroll position
            cleanupOptionsPanel.AutoScrollPosition = new Point(0, customScrollBar.Value);
        }

        private void CustomScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            // Update the panel's scroll position
            cleanupOptionsPanel.AutoScrollPosition = new Point(0, e.NewValue);
        }

        private void UpdateScrollBar()
        {
            if (cleanupOptionsPanel.Controls.Count > 0)
            {
                int contentHeight = 0;
                foreach (Control control in cleanupOptionsPanel.Controls)
                {
                    contentHeight += control.Height + control.Margin.Top + control.Margin.Bottom;
                }

                // Calculate the actual scrollable height
                int scrollableHeight = contentHeight - cleanupOptionsPanel.Height;
                if (scrollableHeight < 0)
                    scrollableHeight = 0;

                // Remove the initial padding from the top to start at the exact top of the list
                cleanupOptionsPanel.Padding = new Padding(16, 0, 16, 16);

                // Set the scrollbar properties
                customScrollBar.Minimum = 0;
                customScrollBar.Maximum = scrollableHeight;
                customScrollBar.LargeChange = 1; // Set to 1 to make the scrollbar thumb behave correctly
                customScrollBar.SmallChange = 20;
                customScrollBar.Visible = scrollableHeight > 0;
                
                // Reset the value to ensure proper initial position
                customScrollBar.Value = 0;
                cleanupOptionsPanel.AutoScrollPosition = new Point(0, 0);
            }
            else
            {
                customScrollBar.Visible = false;
            }
        }

        private void PopulateCleanupOptions()
        {
            // Clear existing controls
            cleanupOptionsPanel.Controls.Clear();
            
            // Add padding to the panel
            cleanupOptionsPanel.Padding = new Padding(16);
            cleanupOptionsPanel.AutoScroll = true;

            // Create option panels for each cleanup option
            foreach (var option in cleanupOptions)
            {
                Panel optionPanel = new Panel
                {
                    Width = cleanupOptionsPanel.Width - 40, // Account for padding and scrollbar
                    Height = 48,
                    Margin = new Padding(0, 0, 0, 8),
                    Tag = option.Key,
                    Cursor = Cursors.Hand
                };

                // Create Material3 style checkbox
                MaterialCheckBox checkBox = new MaterialCheckBox
                {
                    Size = new Size(24, 24),
                    Location = new Point(16, 12), // Center vertically
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent,
                    Tag = option.Key
                };

                // Create label with size information
                string labelText = option.Key;
                
                // Only show size for file/folder-based cleanups, not command-based ones
                if (option.Value.CustomCleanupFunction != null && string.IsNullOrEmpty(option.Value.Path))
                {
                    // Command-based cleanup (like DNS Cache) - don't show size
                    if (!option.Value.Description.Contains("Recycle Bin"))
                    {
                        labelText = option.Key; // No size display
                    }
                    else
                    {
                        // Recycle Bin - show size
                        string sizeText = option.Value.GetFormattedSize();
                        labelText = $"{option.Key} ({sizeText})";
                    }
                }
                else
                {
                    // File/folder-based cleanup - show size
                    string sizeText = option.Value.GetFormattedSize();
                    labelText = $"{option.Key} ({sizeText})";
                }
                
                Label label = new Label
                {
                    Text = labelText,
                    AutoSize = false,
                    Size = new Size(optionPanel.Width - 60, 24),
                    Location = new Point(48, 12), // Center vertically
                    Font = new Font("Segoe UI", 11),
                    ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Add controls to panel
                optionPanel.Controls.Add(checkBox);
                optionPanel.Controls.Add(label);

                // Add click event to the panel
                optionPanel.Click += (sender, e) => 
                {
                    selectedOption = option.Key;
                    UpdateSelectedOptionUI();
                    UpdateDescriptionPanel(option.Key);
                };

                // Add click event to the label (to make the whole row clickable)
                label.Click += (sender, e) => 
                {
                    selectedOption = option.Key;
                    UpdateSelectedOptionUI();
                    UpdateDescriptionPanel(option.Key);
                };

                // Add change event to checkbox
                checkBox.CheckedChanged += (sender, e) => 
                {
                    // Update Select All checkbox state when individual checkboxes change
                    UpdateSelectAllCheckboxState();
                };

                // Add the panel to the flow layout
                cleanupOptionsPanel.Controls.Add(optionPanel);
            }
        }

        private void UpdateSelectAllCheckboxState()
        {
            if (materialSelectAllCheckBox == null || isUpdatingSelectAll) return;

            // Count total checkboxes and checked checkboxes
            int totalCheckboxes = 0;
            int checkedCheckboxes = 0;

            foreach (Control control in cleanupOptionsPanel.Controls)
            {
                if (control is Panel panel)
                {
                    MaterialCheckBox? checkBox = panel.Controls.OfType<MaterialCheckBox>().FirstOrDefault();
                    if (checkBox != null)
                    {
                        totalCheckboxes++;
                        if (checkBox.Checked)
                        {
                            checkedCheckboxes++;
                        }
                    }
                }
            }

            // Set flag to prevent infinite loop
            isUpdatingSelectAll = true;

            // Update Select All checkbox state
            if (checkedCheckboxes == 0)
            {
                materialSelectAllCheckBox.Checked = false;
            }
            else if (checkedCheckboxes == totalCheckboxes)
            {
                materialSelectAllCheckBox.Checked = true;
            }
            else
            {
                // For partial selection, we'll keep it unchecked
                // You could implement indeterminate state here if desired
                materialSelectAllCheckBox.Checked = false;
            }

            // Reset flag
            isUpdatingSelectAll = false;
        }

        private void UpdateSelectedOptionUI()
        {
            // Update the background color of all option panels
            foreach (Control control in cleanupOptionsPanel.Controls)
            {
                if (control is Panel panel)
                {
                    if (panel.Tag.ToString() == selectedOption)
                    {
                        panel.BackColor = IsDarkMode() ? darkSurfaceVariantColor : surfaceVariantColor;
                    }
                    else
                    {
                        panel.BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
                    }
                }
            }
        }

        private void UpdateDescriptionPanel(string optionName)
        {
            if (cleanupOptions.TryGetValue(optionName, out CleanupOption? option) && option != null)
            {
                // Create a rich text description
                descriptionTextBox.Clear();
                descriptionTextBox.SelectionFont = new Font("Segoe UI", 11);
                
                // Add the description text
                string description = option.Description;
                
                // Add the main description
                descriptionTextBox.AppendText(description);
                
                // Add path information if available
                if (!string.IsNullOrEmpty(option.Path))
                {
                    descriptionTextBox.AppendText("\r\n\r\nLocation: " + option.Path);
                    
                    // Add file pattern info if available
                    if (!string.IsNullOrEmpty(option.FilePattern))
                    {
                        descriptionTextBox.AppendText("\r\nFile Pattern: " + option.FilePattern);
                    }
                }
                else if (option.CustomCleanupFunction != null)
                {
                    descriptionTextBox.AppendText("\r\n\r\nThis option uses a system command to perform cleanup.");
                }
                
                // Add risk warning if applicable
                if (option.IsRisky && !string.IsNullOrEmpty(option.RiskMessage))
                {
                    // Add extra line breaks
                    descriptionTextBox.AppendText("\r\n\r\n");
                    
                    // Add the "RISKY:" text in bold red
                    descriptionTextBox.SelectionStart = descriptionTextBox.TextLength;
                    descriptionTextBox.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
                    descriptionTextBox.SelectionColor = Color.Red;
                    descriptionTextBox.AppendText("RISKY: ");
                    
                    // Add the risk message in yellow
                    descriptionTextBox.SelectionStart = descriptionTextBox.TextLength;
                    descriptionTextBox.SelectionFont = new Font("Segoe UI", 11);
                    descriptionTextBox.SelectionColor = Color.FromArgb(255, 204, 0); // Yellow
                    descriptionTextBox.AppendText(option.RiskMessage);
                }
                
                // Reset the selection
                descriptionTextBox.SelectionStart = 0;
                descriptionTextBox.SelectionLength = 0;
                descriptionTextBox.SelectionColor = descriptionTextBox.ForeColor;
                
                // Update the risk warning label visibility
                riskWarningLabel.Visible = option.IsRisky;
            }
        }

        private void InitializeCleanupOptions()
        {
            // Add all cleanup options with their paths, descriptions, and risk levels
            cleanupOptions.Add("Windows Temp", new CleanupOption(
                @"C:\Windows\Temp",
                "Cleans temporary files created by Windows and system applications. These files are used for temporary storage during installation and updates. Windows automatically recreates this directory if needed.",
                false));

            cleanupOptions.Add("User Temp", new CleanupOption(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Local\Temp"),
                "Cleans temporary files specific to your user account. Applications store cache data, installation files, and temporary documents here. These files are safe to remove as applications will recreate them if needed.",
                false));

            cleanupOptions.Add("Prefetch", new CleanupOption(
                @"C:\Windows\Prefetch",
                "Cleans prefetch data which Windows uses to speed up application launches. Windows monitors which applications you use frequently and preloads parts of them for faster startup. The system will automatically rebuild these files over time.",
                false));

            cleanupOptions.Add("Print Spooler", new CleanupOption(
                @"C:\Windows\System32\spool\PRINTERS",
                "Cleans stuck print jobs and printer queue files. This can help resolve printing issues where documents are stuck in the queue or not printing correctly.",
                false));

            cleanupOptions.Add("Windows Update Cache", new CleanupOption(
                @"C:\Windows\SoftwareDistribution\Download",
                "Cleans downloaded Windows Update installation files. After updates are installed, these files are no longer needed. Windows Update will redownload any necessary files if you need to reinstall updates.",
                false));

            cleanupOptions.Add("Thumbnail Cache", new CleanupOption(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Windows\Explorer"),
                "Cleans cached thumbnails for files and folders in File Explorer. Windows creates these thumbnails to speed up browsing and will regenerate them automatically when needed.",
                false,
                "thumbcache_*.db"));

            cleanupOptions.Add("Windows Log Files", new CleanupOption(
                @"C:\Windows\Logs",
                "Cleans Windows diagnostic log files used for troubleshooting system issues. These logs contain information about system events, errors, and application crashes.",
                true,
                "",
                null,
                "May remove logs needed for diagnosing system issues. If you're experiencing problems with your system, you might want to keep these logs for troubleshooting."));

            cleanupOptions.Add("Delivery Optimization Files", new CleanupOption(
                @"C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache",
                "Cleans Windows delivery optimization cache used for updates and Store apps. This cache helps Windows download updates more efficiently by storing parts of updates that can be shared with other devices on your network.",
                false));

            cleanupOptions.Add("Microsoft Edge Cache", new CleanupOption(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\User Data\Default\Cache"),
                "Cleans Microsoft Edge browser cache files. These files store web page content, images, and other media to speed up browsing. Clearing this cache can free up space and may help resolve some browsing issues.",
                false));

            cleanupOptions.Add("Windows Defender Logs", new CleanupOption(
                @"C:\ProgramData\Microsoft\Windows Defender\Scans\History",
                "Cleans Windows Defender antivirus scan history logs. These logs contain information about previous scans, detected threats, and actions taken by Windows Defender.",
                true,
                "",
                null,
                "May remove security history needed for tracking threats. If you're investigating security incidents, you might want to keep these logs."));

            cleanupOptions.Add("Windows Error Reporting", new CleanupOption(
                @"C:\ProgramData\Microsoft\Windows\WER",
                "Cleans Windows Error Reporting files used for crash reports. When applications crash, Windows creates these files to help developers diagnose and fix issues. These files are sent to Microsoft if you choose to report problems.",
                false));

            cleanupOptions.Add("DNS Cache", new CleanupOption(
                "",
                "Flushes DNS resolver cache. The DNS cache stores the locations (IP addresses) of web servers that contain web pages which you have recently viewed. Clearing this cache can help resolve some network connectivity issues.",
                false,
                "",
                () => { Process.Start(new ProcessStartInfo("ipconfig", "/flushdns") { CreateNoWindow = true, UseShellExecute = false }); return true; }));

            cleanupOptions.Add("Recycle Bin", new CleanupOption(
                "",
                "Empties the Recycle Bin. This permanently removes all deleted files currently stored in the Recycle Bin across all drives.",
                true,
                "",
                () => {
                    SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    return true;
                },
                "Permanently deletes all files in the Recycle Bin. These files cannot be recovered after this operation."));
        }

        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

        private void ApplyMaterial3Theme()
        {
            // Apply Material 3 theme to the form and controls
            this.BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
            this.ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor;
            
            // Title panel
            titlePanel.BackColor = primaryColor;
            titleLabel.ForeColor = onPrimaryColor;
            exitButton.BackColor = primaryColor;
            exitButton.ForeColor = onPrimaryColor;
            minimizeButton.BackColor = primaryColor;
            minimizeButton.ForeColor = onPrimaryColor;
            
            // Main panel
            mainPanel.BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
            
            // Cleanup options panel
            cleanupOptionsPanel.BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
            
            // Description panel
            descriptionTextBox.BackColor = IsDarkMode() ? darkSurfaceVariantColor : surfaceVariantColor;
            descriptionTextBox.ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor;
            
            // Clean button
            cleanButton.BackColor = primaryColor;
            cleanButton.ForeColor = onPrimaryColor;
            cleanButton.FlatAppearance.BorderSize = 0;
            cleanButton.FlatStyle = FlatStyle.Flat;
            
            // Risk warning label
            riskWarningLabel.ForeColor = errorColor;
            
            // Progress bar
            progressBar.Style = ProgressBarStyle.Continuous;
            
            // Status label
            statusLabel.ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor;
            
            // Select all checkbox
            selectAllCheckBox.ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor;
            
            // Custom scrollbar
            if (customScrollBar != null)
            {
                customScrollBar.BackColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
                customScrollBar.ThumbColor = primaryColor;
                customScrollBar.BorderColor = IsDarkMode() ? darkSurfaceColor : surfaceColor;
            }
            
            // Replace the standard checkbox with Material3 checkbox for Select All
            ReplaceSelectAllWithMaterialCheckbox();
        }

        private void ReplaceSelectAllWithMaterialCheckbox()
        {
            // Create a new Material3 checkbox
            MaterialCheckBox materialCheckBox = new MaterialCheckBox
            {
                Size = new Size(24, 24),
                Location = new Point(16, 16),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Name = "materialSelectAllCheckBox"
            };
            
            // Store reference to the Select All checkbox
            materialSelectAllCheckBox = materialCheckBox;

            // Create a new label for the text
            Label selectAllLabel = new Label
            {
                Text = "Select All",
                AutoSize = true,
                Location = new Point(48, 16),
                Font = new Font("Segoe UI Semibold", 11),
                ForeColor = IsDarkMode() ? onDarkSurfaceColor : onSurfaceColor,
                Cursor = Cursors.Hand
            };

            // Add click event to the label
            selectAllLabel.Click += (sender, e) => 
            {
                materialCheckBox.Checked = !materialCheckBox.Checked;
            };

            // Connect the new checkbox to the select all functionality
            materialCheckBox.CheckedChanged += (sender, e) => 
            {
                // Only update if not already updating (prevent infinite loop)
                if (!isUpdatingSelectAll)
                {
                    bool isChecked = materialCheckBox.Checked;
                    
                    // Update all checkboxes in the options panel
                    foreach (Control control in cleanupOptionsPanel.Controls)
                    {
                        if (control is Panel panel)
                        {
                            MaterialCheckBox? checkBox = panel.Controls.OfType<MaterialCheckBox>().FirstOrDefault();
                            if (checkBox != null)
                            {
                                checkBox.Checked = isChecked;
                            }
                        }
                    }
                }
            };

            // Remove the original checkbox
            selectAllCheckBox.Visible = false;
            selectAllCheckBox.Enabled = false;

            // Add the new controls
            optionsPanel.Controls.Add(materialCheckBox);
            optionsPanel.Controls.Add(selectAllLabel);

            // Bring them to the front
            materialCheckBox.BringToFront();
            selectAllLabel.BringToFront();
        }

        private bool IsDarkMode()
        {
            // Check if Windows is in dark mode
            // For now, we'll default to dark mode
            return true;
        }

        protected override void WndProc(ref Message m)
        {
            // Enable dragging the form by clicking on the title panel
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (m.Result.ToInt32() == HTCLIENT)
                {
                    Point screenPoint = new Point(m.LParam.ToInt32());
                    Point clientPoint = this.PointToClient(screenPoint);
                    
                    // Only allow dragging from the title panel
                    if (titlePanel.Bounds.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)HTCAPTION;
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }

        // Method to handle form dragging
        private void TitlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xA1, 0x2, 0);
            }
        }

        // Clean the selected items
        private async void CleanButton_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            statusLabel.Visible = true;
            cleanButton.Enabled = false;
            selectAllCheckBox.Enabled = false;

            int totalItems = 0;
            int processedItems = 0;
            
            // Count selected items
            foreach (Control control in cleanupOptionsPanel.Controls)
            {
                if (control is Panel panel)
                {
                    MaterialCheckBox? checkBox = panel.Controls.OfType<MaterialCheckBox>().FirstOrDefault();
                    if (checkBox != null && checkBox.Checked)
                    {
                        totalItems++;
                    }
                }
            }

            if (totalItems == 0)
            {
                statusLabel.Text = "No items selected for cleanup.";
                progressBar.Visible = false;
                cleanButton.Enabled = true;
                selectAllCheckBox.Enabled = true;
                return;
            }

            progressBar.Maximum = totalItems;
            progressBar.Value = 0;

            // Process each selected item
            foreach (Control control in cleanupOptionsPanel.Controls)
            {
                if (control is Panel panel)
                {
                    MaterialCheckBox? checkBox = panel.Controls.OfType<MaterialCheckBox>().FirstOrDefault();
                    if (checkBox != null && checkBox.Checked)
                    {
                        string optionName = panel.Tag.ToString()!;
                        statusLabel.Text = $"Cleaning {optionName}...";
                        
                        // Perform the cleanup operation asynchronously
                        bool success = await Task.Run(() => CleanOption(cleanupOptions[optionName]));
                        
                        // Update progress
                        processedItems++;
                        progressBar.Value = processedItems;
                        
                        if (success)
                        {
                            statusLabel.Text = $"Cleaned {optionName} successfully.";
                        }
                        else
                        {
                            statusLabel.Text = $"Failed to clean {optionName}.";
                        }
                        
                        // Small delay to show the status
                        await Task.Delay(500);
                    }
                }
            }

            // Cleanup complete
            statusLabel.Text = "Cleanup complete!";
            
            // Immediately refresh size displays to show updated values
            RefreshSizeDisplays();
            
            cleanButton.Enabled = true;
            selectAllCheckBox.Enabled = true;
        }

        private bool CleanOption(CleanupOption option)
        {
            try
            {
                // If custom cleanup function exists, use it
                if (option.CustomCleanupFunction != null)
                {
                    return option.CustomCleanupFunction();
                }

                // Standard file/directory cleanup
                if (string.IsNullOrEmpty(option.Path))
                    return true;

                if (Directory.Exists(option.Path))
                {
                    if (string.IsNullOrEmpty(option.FilePattern))
                    {
                        // Clean all files in directory
                        foreach (string file in Directory.GetFiles(option.Path, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { /* Continue if file is in use */ }
                        }

                        // Clean empty subdirectories
                        foreach (string dir in Directory.GetDirectories(option.Path))
                        {
                            try
                            {
                                Directory.Delete(dir, true);
                            }
                            catch { /* Continue if directory is in use */ }
                        }
                    }
                    else
                    {
                        // Clean only specific file pattern
                        foreach (string file in Directory.GetFiles(option.Path, option.FilePattern, SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { /* Continue if file is in use */ }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SelectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // This method is no longer used since we're using MaterialCheckBox
            // The functionality is handled in the ReplaceSelectAllWithMaterialCheckbox method
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }

    // Class to store cleanup option details
    public class CleanupOption
    {
        public string Path { get; set; }
        public string Description { get; set; }
        public bool IsRisky { get; set; }
        public string FilePattern { get; set; }
        public Func<bool>? CustomCleanupFunction { get; set; }
        public string? RiskMessage { get; set; }

        public CleanupOption(string path, string description, bool isRisky, string filePattern = "", Func<bool>? customCleanupFunction = null, string? riskMessage = null)
        {
            Path = path;
            Description = description;
            IsRisky = isRisky;
            FilePattern = filePattern;
            CustomCleanupFunction = customCleanupFunction;
            RiskMessage = riskMessage;
        }

        public long CalculateSize()
        {
            try
            {
                // For custom cleanup functions (like DNS cache or Recycle Bin), return 0
                if (CustomCleanupFunction != null)
                {
                    if (Path == "")
                    {
                        // Special handling for Recycle Bin
                        if (Description.Contains("Recycle Bin"))
                        {
                            return CalculateRecycleBinSize();
                        }
                        return 0; // DNS Cache and other command-based cleanups
                    }
                }

                if (string.IsNullOrEmpty(Path) || !Directory.Exists(Path))
                    return 0;

                long totalSize = 0;

                if (!string.IsNullOrEmpty(FilePattern))
                {
                    // Calculate size for specific file pattern
                    totalSize = CalculatePatternSize(Path, FilePattern);
                }
                else
                {
                    // Calculate size for entire directory
                    totalSize = CalculateDirectorySize(Path);
                }

                return totalSize;
            }
            catch
            {
                return 0; // Return 0 if there's an error (e.g., access denied)
            }
        }

        private long CalculateDirectorySize(string dirPath)
        {
            long size = 0;
            
            try
            {
                // Get all files in directory
                string[] files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    catch
                    {
                        // Skip files that can't be accessed
                        continue;
                    }
                }
            }
            catch
            {
                // Return current size if there's an error
            }

            return size;
        }

        private long CalculatePatternSize(string dirPath, string pattern)
        {
            long size = 0;
            
            try
            {
                string[] files = Directory.GetFiles(dirPath, pattern, SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    catch
                    {
                        // Skip files that can't be accessed
                        continue;
                    }
                }
            }
            catch
            {
                // Return current size if there's an error
            }

            return size;
        }

        private long CalculateRecycleBinSize()
        {
            try
            {
                long totalSize = 0;
                DriveInfo[] drives = DriveInfo.GetDrives();
                
                foreach (DriveInfo drive in drives)
                {
                    if (drive.DriveType == DriveType.Fixed)
                    {
                        string recycleBinPath = System.IO.Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin");
                        if (Directory.Exists(recycleBinPath))
                        {
                            // Only count user-accessible recycle bin files
                            totalSize += CalculateUserRecycleBinSize(recycleBinPath);
                        }
                    }
                }
                
                return totalSize;
            }
            catch
            {
                return 0;
            }
        }

        private long CalculateUserRecycleBinSize(string recycleBinPath)
        {
            try
            {
                long size = 0;
                
                // Get the current user's SID directory in the recycle bin
                string currentUserSid = System.Security.Principal.WindowsIdentity.GetCurrent().User?.Value ?? "";
                if (!string.IsNullOrEmpty(currentUserSid))
                {
                    string userRecycleBinPath = System.IO.Path.Combine(recycleBinPath, currentUserSid);
                    if (Directory.Exists(userRecycleBinPath))
                    {
                        // Only count actual deleted files, not system metadata
                        string[] files = Directory.GetFiles(userRecycleBinPath, "$R*", SearchOption.TopDirectoryOnly);
                        foreach (string file in files)
                        {
                            try
                            {
                                FileInfo fileInfo = new FileInfo(file);
                                size += fileInfo.Length;
                            }
                            catch
                            {
                                // Skip files that can't be accessed
                                continue;
                            }
                        }
                    }
                }
                
                return size;
            }
            catch
            {
                return 0;
            }
        }

        public string GetFormattedSize()
        {
            long bytes = CalculateSize();
            return FormatBytes(bytes);
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";
            
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = bytes;
            
            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }
            
            return $"{size:F1} {suffixes[suffixIndex]}";
        }
    }

    // Custom scrollbar control
    public class CustomScrollBar : Control
    {
        private bool isDragging = false;
        private int thumbPosition = 0;
        private int thumbHeight = 30;
        private int _value = 0;
        private int _minimum = 0;
        private int _maximum = 100;
        private int _largeChange = 10;
        private int _smallChange = 1;
        private Color _thumbColor = Color.FromArgb(103, 80, 164); // Primary color (deep purple)
        private Color _borderColor = Color.FromArgb(28, 27, 31); // Dark surface color
        private int dragStartOffset = 0; // Track where in the thumb the user clicked

        public event ScrollEventHandler? Scroll;

        public CustomScrollBar()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, true);
            
            // Set default colors to match Material3 theme
            BackColor = Color.FromArgb(28, 27, 31); // Dark surface color
            ForeColor = Color.FromArgb(230, 225, 229); // On dark surface color
        }

        public int Value
        {
            get => _value;
            set
            {
                if (value < _minimum)
                    value = _minimum;
                if (value > _maximum)
                    value = _maximum;
                
                if (_value != value)
                {
                    _value = value;
                    OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, _value));
                    UpdateThumbPosition();
                    Invalidate();
                }
            }
        }

        public int Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum)
                    Value = _minimum;
                UpdateThumbPosition();
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum)
                    Value = _maximum;
                UpdateThumbPosition();
                Invalidate();
            }
        }

        public int LargeChange
        {
            get => _largeChange;
            set
            {
                _largeChange = value;
                UpdateThumbPosition();
                Invalidate();
            }
        }

        public int SmallChange
        {
            get => _smallChange;
            set => _smallChange = value;
        }

        public Color ThumbColor
        {
            get => _thumbColor;
            set
            {
                _thumbColor = value;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        private void UpdateThumbPosition()
        {
            if (_maximum == _minimum)
                thumbPosition = 0;
            else
            {
                int trackHeight = Height;
                
                // Calculate thumb height based on the proportion of visible content
                float thumbRatio = Math.Min(1.0f, (float)trackHeight / (_maximum + trackHeight));
                thumbHeight = Math.Max((int)(trackHeight * thumbRatio), 20);
                
                // Calculate thumb position based on current value
                if (_maximum == 0)
                {
                    thumbPosition = 0;
                }
                else
                {
                    float valueRatio = (float)_value / _maximum;
                    thumbPosition = (int)(valueRatio * (trackHeight - thumbHeight));
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Draw background
            using (SolidBrush backBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backBrush, ClientRectangle);
            }
            
            // Calculate thumb rectangle
            int thumbWidth = Width - 4; // Slightly narrower for better appearance
            Rectangle thumbRect = new Rectangle(
                2, // Center horizontally with small padding
                thumbPosition,
                thumbWidth,
                thumbHeight);
            
            // Draw the thumb with rounded corners
            if (Enabled && thumbHeight > 0 && Height > 0)
            {
                Color thumbFillColor = ThumbColor;
                
                // Apply transparency for hover/press effect
                if (isDragging)
                {
                    // Darker when dragging
                    thumbFillColor = Color.FromArgb(
                        Math.Max(0, thumbFillColor.A - 40),
                        thumbFillColor.R,
                        thumbFillColor.G,
                        thumbFillColor.B);
                }
                
                // Draw rounded thumb
                using (SolidBrush thumbBrush = new SolidBrush(thumbFillColor))
                {
                    g.FillRoundedRectangle(thumbBrush, thumbRect, 4);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y >= thumbPosition && e.Y <= thumbPosition + thumbHeight)
                {
                    isDragging = true;
                    dragStartOffset = e.Y - thumbPosition; // Store where in the thumb the user clicked
                    Capture = true;
                }
                else
                {
                    // Click on track
                    if (e.Y < thumbPosition)
                    {
                        // Click above thumb - page up
                        Value -= _smallChange * 5;
                    }
                    else
                    {
                        // Click below thumb - page down
                        Value += _smallChange * 5;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                int trackHeight = Height - thumbHeight;
                if (trackHeight <= 0)
                    return;
                
                // Calculate the new thumb position accounting for where the user grabbed it
                int newThumbPosition = e.Y - dragStartOffset;
                
                // Ensure the thumb stays within bounds
                newThumbPosition = Math.Max(0, Math.Min(trackHeight, newThumbPosition));
                
                // Convert the thumb position to a value
                float ratio = (float)_maximum / trackHeight;
                int newValue = (int)(newThumbPosition * ratio);
                
                Value = newValue;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
            Capture = false;
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateThumbPosition();
        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            if (pen == null) throw new ArgumentNullException(nameof(pen));

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            if (brush == null) throw new ArgumentNullException(nameof(brush));

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    public class MaterialCheckBox : Control
    {
        private bool _checked = false;
        private bool _mouseOver = false;
        private bool _mouseDown = false;
        private readonly Color _checkedColor;
        private readonly Color _uncheckedColor;
        private readonly Color _checkmarkColor;

        public event EventHandler? CheckedChanged;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnCheckedChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public MaterialCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            _checkedColor = Color.FromArgb(103, 80, 164);      // Deep purple (primary color)
            _uncheckedColor = Color.FromArgb(73, 69, 79);      // Dark surface variant
            _checkmarkColor = Color.White;
            
            Size = new Size(24, 24);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Calculate checkbox dimensions
            int boxSize = Math.Min(Width, Height) - 4;
            int x = (Width - boxSize) / 2;
            int y = (Height - boxSize) / 2;
            
            Rectangle boxRect = new Rectangle(x, y, boxSize, boxSize);
            
            // Draw the checkbox background
            if (Checked)
            {
                using (SolidBrush brush = new SolidBrush(_checkedColor))
                {
                    g.FillRoundedRectangle(brush, boxRect, 4);
                }
                
                // Draw checkmark
                using (Pen pen = new Pen(_checkmarkColor, 2))
                {
                    // Calculate checkmark points
                    int checkWidth = boxSize - 8;
                    int checkHeight = boxSize - 10;
                    
                    int startX = x + 4;
                    int startY = y + boxSize / 2;
                    
                    int middleX = startX + checkWidth / 3;
                    int middleY = startY + checkHeight / 3;
                    
                    int endX = startX + checkWidth;
                    int endY = startY - checkHeight / 2;
                    
                    // Draw the checkmark
                    g.DrawLine(pen, startX, startY, middleX, middleY);
                    g.DrawLine(pen, middleX, middleY, endX, endY);
                }
            }
            else
            {
                // Draw empty box with border
                using (Pen pen = new Pen(_uncheckedColor, 2))
                {
                    g.DrawRoundedRectangle(pen, boxRect, 4);
                }
            }
            
            // Draw focus/hover effect
            if (_mouseOver)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, _checkedColor)))
                {
                    g.FillEllipse(brush, new Rectangle(x - 4, y - 4, boxSize + 8, boxSize + 8));
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseOver = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseOver = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = true;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _mouseDown)
            {
                _mouseDown = false;
                if (ClientRectangle.Contains(e.Location))
                {
                    Checked = !Checked;
                }
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }
    }
} 