# XNull Cleanup Tool

A modern, Material 3-themed Windows cleanup utility that helps you free up disk space by removing temporary files and other unnecessary data.

## Features

- **Modern Material 3 UI**: Clean, modern interface with a dark theme following Material 3 design guidelines
  - Custom Material 3 checkboxes with hover effects and smooth animations
  - Custom Material 3 themed scrollbar for the options list
  - Proper elevation and surface treatments with rounded corners
  - Consistent typography using Segoe UI font family
  - Borderless window with custom title bar and window controls
- **13 Comprehensive Cleanup Options**: Clean various parts of Windows including:
  - **Windows Temp Files** (`C:\Windows\Temp`) - System temporary files
  - **User Temp Files** (`%USERPROFILE%\AppData\Local\Temp`) - User-specific temporary files
  - **Prefetch Data** (`C:\Windows\Prefetch`) - Application launch optimization files
  - **Print Spooler Files** (`C:\Windows\System32\spool\PRINTERS`) - Stuck print jobs
  - **Windows Update Cache** (`C:\Windows\SoftwareDistribution\Download`) - Downloaded update files
  - **Thumbnail Cache** (`%LOCALAPPDATA%\Microsoft\Windows\Explorer`) - File Explorer thumbnails
  - **Windows Log Files** (`C:\Windows\Logs`) - System diagnostic logs ⚠️ **RISKY**
  - **Delivery Optimization Files** (`C:\Windows\ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache`) - Windows update optimization cache
  - **Microsoft Edge Cache** (`%LOCALAPPDATA%\Microsoft\Edge\User Data\Default\Cache`) - Browser cache files
  - **Windows Defender Logs** (`C:\ProgramData\Microsoft\Windows Defender\Scans\History`) - Antivirus scan history ⚠️ **RISKY**
  - **Windows Error Reporting Files** (`C:\ProgramData\Microsoft\Windows\WER`) - Crash report files
  - **DNS Cache** - Network domain name resolution cache (uses `ipconfig /flushdns`)
  - **Recycle Bin** - Permanently empties all recycle bins ⚠️ **RISKY**

- **Enhanced Risk Warnings**: 
  - Clear visual indicators for potentially risky cleanup options
  - "RISKY" text in bold red followed by yellow warning text
  - Separated from main description for better readability
- **Detailed Interactive Descriptions**: 
  - Click any cleanup option to see comprehensive information about what it does
  - Shows exact file locations and patterns being cleaned
  - Explains how the system uses these files and what happens after cleaning
  - Contextual description panel that updates based on selection
- **Async Progress Tracking**: 
  - Visual progress bar during cleanup operations with real-time status updates
  - Asynchronous cleanup process that doesn't freeze the UI
  - Individual status messages for each cleanup operation
- **Smart Selection Features**:
  - Material 3 styled "Select All" checkbox for quick selection
  - Individual Material 3 checkboxes for each cleanup option
  - Visual selection highlighting with Material 3 surface colors

## UI Features

- **Material 3 Design Language**: Complete implementation with proper spacing, typography, and elevation
- **Dark Theme**: Hardcoded dark mode following Material 3 specifications
- **Split Panel Layout**: Options list on the left, contextual descriptions on the right
- **Interactive Elements**: 
  - Highlighted selection states for better user experience
  - Custom Material 3 checkboxes with hover effects
  - Borderless window with custom draggable title bar
  - Custom minimize and exit buttons in the title bar
- **Custom Scrolling**: Material 3 themed scrollbar for the cleanup options list
- **Rich Text Descriptions**: Formatted text with colors and styling for better readability
- **Responsive Design**: Proper anchoring and layout management for window resizing

## Requirements

- Windows 10 or later
- .NET 6.0 Windows Runtime
- Administrator privileges (automatically requested via UAC manifest)
- Minimum screen resolution: 800x600 (optimized for 1024x768 and higher)

## Installation

1. Download the latest release from the Releases section
2. Extract the files to any location on your computer
3. **Important**: Add a valid .ico file named "cleanup.ico" to the Resources directory (see Resources/README.txt for details)
4. Run `XNullCleanup.exe` (Administrator privileges will be requested automatically via Windows UAC)

### Icon Setup
The application requires a custom icon file to display properly in the taskbar:
- Place a .ico file named "cleanup.ico" in the Resources folder
- The icon should be at least 32x32 pixels in size
- You can convert PNG/JPG images to ICO format using online converters

## Usage

1. Launch the application (will request administrator privileges automatically)
2. **Browse cleanup options**: Click on any item in the left panel to see detailed information
3. **Select items to clean**: Use individual checkboxes or "Select All" for convenience
4. **Review descriptions and warnings**: Check the right panel for detailed explanations and risk warnings
5. **Start cleanup**: Click "Clean Selected Items" to begin the process
6. **Monitor progress**: Watch the progress bar and status messages during cleanup
7. **Review results**: Check the final status messages for any issues or completion confirmation

### Interface Guide
- **Left Panel**: List of all available cleanup options with checkboxes
- **Right Panel**: Detailed descriptions, file locations, and risk warnings for selected items
- **Bottom Section**: Progress bar, status messages, and action buttons
- **Title Bar**: Custom minimize and exit buttons (click and drag to move window)

## Important Notes

- **Administrator Privileges**: This application requires administrator privileges to access and clean system directories (automatically handled via app.manifest)
- **Risky Options**: Three cleanup options are marked as risky:
  - **Windows Log Files**: May remove logs needed for diagnosing system issues
  - **Windows Defender Logs**: May remove security history needed for tracking threats
  - **Recycle Bin**: Permanently deletes all files (cannot be recovered)
- **Safety Features**: 
  - Files in use by the system are automatically skipped to prevent system instability
  - Each operation shows detailed information before execution
  - Asynchronous processing prevents UI freezing during cleanup
- **Performance Impact**: Some cleanup operations may temporarily slow down your system as Windows rebuilds caches and optimizations

## Technical Details

### Architecture
- **Framework**: .NET 6.0 Windows Forms Application
- **UI**: Custom Material 3 implementation with WinForms
- **Threading**: Asynchronous Task-based cleanup operations
- **Security**: UAC elevation via application manifest

### Custom Components
- **MaterialCheckBox**: Custom Material 3 styled checkbox control
- **CustomScrollBar**: Material 3 themed scrollbar with smooth scrolling
- **CleanupOption Class**: Encapsulates cleanup logic and metadata
- **Graphics Extensions**: Helper methods for rounded rectangles and Material 3 styling

## Building from Source

### Prerequisites
- Visual Studio 2022 or later with .NET 6.0 Windows development workload
- Windows 10 SDK (for Windows Forms support)

### Build Steps
1. Clone this repository
2. Open `XNullCleanup.sln` in Visual Studio
3. Restore NuGet packages (automatic in VS 2022)
4. Set build configuration to Release for distribution
5. Build the solution (Ctrl+Shift+B)
6. Add the required cleanup.ico file to the Resources directory
7. The executable will be in `bin\Release\net6.0-windows\`

### Project Structure
```
XNull Cleanup Tool/
├── MainForm.cs              # Main application logic and UI
├── MainForm.Designer.cs     # Auto-generated UI layout
├── Program.cs               # Application entry point
├── XNullCleanup.csproj     # Project configuration
├── app.manifest            # UAC elevation configuration
└── Resources/
    ├── cleanup.ico         # Application icon (user-provided)
    └── README.txt          # Icon setup instructions
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://raw.githubusercontent.com/xnull-eu/xnull-cleanup-tool/refs/heads/main/LICENSE) file for details. 
