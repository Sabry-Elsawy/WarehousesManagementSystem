# Profile Picture Feature - Implementation Notes

## Changes Made

### 1. Database Schema
**Added to ApplicationUser:**
- `ProfilePicturePath` (string, nullable) - Stores relative path to uploaded image

**Migration Required:** Yes - Add `ProfilePicturePath` column to `AspNetUsers` table

### 2. Files Modified

**ApplicationUser.cs**
- Added `ProfilePicturePath` property

**ProfileViewModel.cs**
- Added `ProfilePicture` (IFormFile) for upload
- Added `ProfilePicturePath` (string) for display

**ProfileController.cs**
- Added `IWebHostEnvironment` dependency injection
- Implemented file upload logic with validation:
  - File type validation (.jpg, .jpeg, .png, .gif)
  - File size validation (max 5MB)
  - Old file deletion before new upload
  - Unique filename generation
  - Saves to `/wwwroot/uploads/profiles/`

**Profile/Index.cshtml**
- Displays profile picture if available, otherwise shows initials
- Fixed User Info section to show email and phone number

**Profile/Edit.cshtml**
- Added profile picture upload field
- Shows current picture preview
- Set form `enctype="multipart/form-data"`

### 3. Directory Structure
Profile pictures are saved to:
```
wwwroot/
  uploads/
    profiles/
      {userId}_{guid}.{ext}
```

The directory is auto-created if it doesn't exist.

### 4. Migration Command

```powershell
cd d:\DEPI\GRD_PR\WMS\WarehousesManagementSystem\WMS_DEPI_GRAD

dotnet ef migrations add AddProfilePicture --project ..\WMS.DL\WMS.DAL.csproj --startup-project .\WMS_DEPI_GRAD.csproj

dotnet ef database update --project ..\WMS.DL\WMS.DAL.csproj --startup-project .\WMS_DEPI_GRAD.csproj
```

### 5. Features
- ✅ Upload profile picture (JPG, PNG, GIF)
- ✅ File size validation (max 5MB)
- ✅ Auto-delete old picture when uploading new one
- ✅ Display picture in profile header
- ✅ Fallback to initials if no picture
- ✅ Preview current picture in edit form
- ✅ Display email and phone in profile header

### 6. User Info Display Fixed
The profile header now shows:
- Name
- Role badge  
- Email (with envelope icon)
- Phone number (with phone icon)

All information is displayed correctly with proper icons and styling.
