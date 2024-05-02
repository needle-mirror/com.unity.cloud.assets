# Apple privacy manifest
To publish applications for iOS, iPadOS, tvOS, and visionOS platforms on the App Store, you must include a [privacy manifest file](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files) in your application as per [Apple’s privacy policy](https://www.apple.com/legal/privacy/en-ww/). 

> [!NOTE]
> **Note**:
For information on creating a privacy manifest file to include in your application, refer to [Apple’s privacy manifest policy requirements](https://docs.unity3d.com/Manual/apple-privacy-manifest-policy.html).

The [PrivacyInfo.xcprivacy](#PrivacyInfo.xcprivacy) manifest file outlines the required information, ensuring transparency in accordance with user privacy practices. This file lists the [types of data](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_data_use_in_privacy_manifests) that your Unity applications, third-party SDKs, packages, and plug-ins collect, and the reasons for using certain [required reason API](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api) (Apple documentation) categories. Apple also requires that certain domains be declared as [tracking](https://developer.apple.com/app-store/user-privacy-and-data-use/) (Apple documentation); these domains might be blocked unless a user provides consent. 
> [!WARNING]
> **Important**: If your privacy manifest doesn’t declare the use of the required reason API by you or third-party SDKs, the App Store might reject your application. Read more about the [required reason API](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files/describing_use_of_required_reason_api) in Apple’s documentation. 

> [!NOTE]
> Note: The Unity Cloud Assets package is dependent on the following services. Refer to their manifest files for applicable data practices.
> * `com.unity.cloud.common`
>
> `NSPrivacyCollectedDataTypeOtherUserContent` refers to content provided by users.

The privacy manifest for Unity Cloud Assets is available from `1.1.0`.

## PrivacyInfo.xcprivacy
The following code sample contains the PrivacyInfo.xcprivacy manifest for Unity Cloud Assets. This file is also available in the SDK.
To identify the data that this SDK collects and the purpose for collecting it, refer to the following keys:
* `NSPrivacyCollectedDataType`
* `NSPrivacyCollectedDataTypePurposes`

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>NSPrivacyCollectedDataTypes</key>
	<array>
		<dict>
			<key>NSPrivacyCollectedDataType</key>
			<string>NSPrivacyCollectedDataTypeCoarseLocation</string>
			<key>NSPrivacyCollectedDataTypeLinked</key>
			<true/>
			<key>NSPrivacyCollectedDataTypeTracking</key>
			<false/>
			<key>NSPrivacyCollectedDataTypePurposes</key>
			<array>
				<string>NSPrivacyCollectedDataTypePurposeAppFunctionality</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyCollectedDataType</key>
			<string>NSPrivacyCollectedDataTypeOtherUsageData</string>
			<key>NSPrivacyCollectedDataTypeLinked</key>
			<true/>
			<key>NSPrivacyCollectedDataTypeTracking</key>
			<false/>
			<key>NSPrivacyCollectedDataTypePurposes</key>
			<array>
				<string>NSPrivacyCollectedDataTypePurposeAnalytics</string>
				<string>NSPrivacyCollectedDataTypePurposeOther</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyCollectedDataType</key>
			<string>NSPrivacyCollectedDataTypeOtherUserContent</string>
			<key>NSPrivacyCollectedDataTypeLinked</key>
			<true/>
			<key>NSPrivacyCollectedDataTypeTracking</key>
			<false/>
			<key>NSPrivacyCollectedDataTypePurposes</key>
			<array>
				<string>NSPrivacyCollectedDataTypePurposeAnalytics</string>
				<string>NSPrivacyCollectedDataTypePurposeAppFunctionality</string>
			</array>
		</dict>
		<dict>
			<key>NSPrivacyCollectedDataType</key>
			<string>NSPrivacyCollectedDataTypeUserID</string>
			<key>NSPrivacyCollectedDataTypeLinked</key>
			<true/>
			<key>NSPrivacyCollectedDataTypeTracking</key>
			<false/>
			<key>NSPrivacyCollectedDataTypePurposes</key>
			<array>
				<string>NSPrivacyCollectedDataTypePurposeAnalytics</string>
				<string>NSPrivacyCollectedDataTypePurposeAppFunctionality</string>
			</array>
		</dict>
	</array>
</dict>
</plist>

```
