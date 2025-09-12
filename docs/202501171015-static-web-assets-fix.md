# Static Web Assets Build Error Fix

**Date**: 2025-01-17 10:15 UTC
**Issue**: MSB4057 and static web assets resolution errors during build

## Problem Description

The project was experiencing build errors related to static web assets resolution:

```
System.InvalidOperationException: No file exists for the asset at either location 'Z:\Desenvolvimento\sufficit-telephony-jssip\src\wwwroot\jssip-service.min.js' or 'wwwroot\jssip-service.min.js'.
```

The error occurred because:
1. The `IncludeGeneratedStaticFiles` target was trying to include minified files that may not exist at build time
2. There was a dependency on a non-existent `BundlerMinify` target
3. The `ResolveStaticWebAssetsInputsDependsOn` was causing timing issues

## Solution Applied

### Removed Problematic Elements
- Removed `ResolveStaticWebAssetsInputsDependsOn` property that was causing timing conflicts
- Removed custom `IncludeGeneratedStaticFiles` target that had dependency issues
- Removed dependency on non-existent `BundlerMinify` target

### Applied Standard Approach
- Used standard `<Content Include="">` elements for existing JavaScript files
- Added conditional `<ItemGroup>` for minified files that only includes them when they exist:
  ```xml
  <ItemGroup Condition="Exists('wwwroot/jssip-service.min.js') AND Exists('wwwroot/jssip-sessions.min.js')">
      <Content Include="wwwroot/jssip-service.min.js" />
      <Content Include="wwwroot/jssip-sessions.min.js" />
  </ItemGroup>
  ```

## Files Modified
- `sufficit-telephony-jssip/src/Sufficit.Telephony.JsSIP.csproj`

## Result
- Build errors related to static web assets resolved
- BuildBundlerMinifierPlus can still generate minified files when needed
- No more timing conflicts during build process
- Conditional inclusion ensures minified files are only included when they actually exist

## Technical Notes
- The BuildBundlerMinifierPlus package handles the actual minification based on `bundleconfig.json`
- The minified files are generated during build and conditionally included in the output
- This approach avoids the timing issues that occurred with the custom target approach