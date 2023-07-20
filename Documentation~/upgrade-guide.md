# Upgrading

## [0.2.0] - 2023-07-07
- `ServiceHostConfiguration` has been deprecated and replaced with `IServiceHostResolver`. 
  - Use `UnityServiceHostResolverFactory.Create()` to create a default `ServiceHostResolver`.
