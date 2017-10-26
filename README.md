# IntegrationUtility
A tool used to integrate data with LinkGreen

## Development Setup Instructions (Windows Visual Studio)
1. Clone
2. Open in VS
3. Clean solution (just to be safe)
4. Build solution
5. Start

## Setup Application Insights
1. Optionally add "ApplicationInsights.config" to the solution's .git/info/exclude so that it's local changes won't be tracked
2. Replace {YOUR-KEY-HERE} in ApplicationInsights.config with your InstrumentationKey
3. Set App.config "ApplicationInsights" setting to "1" (true)

## Installation Wizard
1. Open solution
2. Set LinkGreenODBCUtility project to "Release"
3. Right click "SetupLinkGreenOdbcUtility" project > Build
4. Navigate to project folder > SetupLinkGreenOdbcUtility
5. Copy/Zip "Release" folder
6. Run Setup.exe
