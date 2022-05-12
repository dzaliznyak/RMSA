# Realtime Math Signal Analizer (RMSA)

This application is designed for experiments in signal processing, analysis, and visualization. It cannot be considered as a standalone tool yet, but rather as a starting point where you can add your own data format or processing algorithm.
The application is developed on the .Net 6.0 platform, WPF and SkiaSharp are used as a graphics library.

## Main Window
![Application Main Window](/Docs/app.png?raw=true "Main Window")

## Channels

The program can visualize up to 8 channels simultaneously. Each channel has its own color. If your data source contains more than one channel, then controls will be automatically added to the toolbar to toggle the visibility of any of the channels and to select the active channel. Only one channel can be selected as active. For this channel, the values on the X and Y axes will be displayed, and you can also use the mouse to move the borders of the data window and scroll the mouse to zoom the data signal.

## Scaling
Scaling is performed only along the Y-axis by scrolling the mouse wheel. Each channel scales independently of the others. To scale a channel, you first need to make it active.
The `Auto Zoom` button turns on the automatic scaling mode - each channel is individually scaled so that the maximum signal value is at the upper border of the graph.
If scaling along the X-axis is required, other approaches can be used - change the `Frame Width` or apply the `NOP` algorithm. In the second case in the result window, you can observe the fragment of the input signal stretched along the X-axis.

## Data Source Settings

**Data Source**: can be `File`, `COM Port`, or sinusoid `Signal Generator` function. Depending on the data source the settings are changed but the following parameters are common for all data source types.

**Data Format**: here you can select some of the predefined data formats. But mostly you will need to write your own data parser. Please see `Rmsa.Core/DataSource/DataParser` for examples. 


### Data Mode: 

**All data mode**: This mode allows you to see the entire contents of the file at once. This mode is not applicable to `COM Port` or `Signal Generator` data sources. 

**Stream data mode**: This mode displays new data as soon as it arrives, shifting the previous data to the left. Data rate depends on `Frame Width` and `Sampling Rate`:

![Stream Data Mode](/Docs/stream_mode.gif?raw=true "Stream Mode")

**Frame data mode**: This mode displays data frame by frame. Frame rate depends on `Frame Width` and `Sampling Rate`:

![Frame Data Mode](/Docs/frame_mode.gif?raw=true "Frame Mode")

**Frame Width**: The number of samples for displaying the input data. This parameter is not related to screen pixels or the size of the application window, it only controls how many input data points will be displayed at one time.

**Sampling Rate**: the number of points (samples) of the input signal received per second. You should set it to match your ADC sampling rate.

## Data Processing Settings
**Algorithm**: can be FFT (Fast Fourier Transform), DFT (Digital Fourier Transform), or NOP (No Operation). There are many good articles on the topic of Fourier transforms, and I will not cover this topic here. Use the source code as a starting point for your own experiments, add new algorithms. The `NOP` algorithm can also be used to scale the input signal. DSPLib from Steve Hageman was used as an FFT implementation (https://www.codeproject.com/Articles/1107480/DSPLib-FFT-DFT-Fourier-Transform-Library-for-NET-6).

**Window**: transformations are not applied to all data at once, but only to those in the window. This window is displayed on the screen as two vertical lines of the same color as the channel color. But besides the fact that the window separates the data we need from the entire stream, it can also transform it. In most cases, applying the window function brings the signal at the edges of the window to zero or nearly zero. Here's an example of applying the `Hamming` window to a sine wave:

![Hamming Window](/Docs/hamming_window.png?raw=true "Hamming Window")

**Window Width**: the width of the window in points (samples). This value must be a power of 2 for the FFT algorithm to work correctly.

**Zero Padding**: padding the window data with zeros on the right. If the FFT algorithm is used, then `Window Width` + `Zero Padding` in total must be a power of 2, otherwise the program will generate an error.

**Result Type**: while the input signal is represented as an array of `doubles`, after conversion we get an array of the same length, but complex numbers. The `Result Type` determines which part of the complex number should be displayed on the graph. For example, in Fourie analysis, we choose `Magnitude` to display the frequency component of the signal, and `Phase` for the phase component. You can also choose the pure real or imaginary part of a complex number.

