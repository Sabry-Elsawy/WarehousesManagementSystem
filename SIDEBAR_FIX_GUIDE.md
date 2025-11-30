# Layout Sidebar Fixes - Complete Solution

## Problem
The sidebar collapse functionality needs:
1. Fixed alignment
2. Smooth collapse animation  
3. Chevron icon rotation on expand/collapse
4. Active link highlighting
5. Updated Procurement links

## Solution

### 1. Update Procurement Links in _Layout.cshtml

Replace lines 69-71 with:
```cshtml
<li class="nav-item"><a class="nav-link" href="/PurchaseOrder/Index"><span class="nav-text">Purchase Orders</span></a></li>
<li class="nav-item"><a class="nav-link" href="/ASN/Index"><span class="nav-text">ASNs</span></a></li>
<li class="nav-item"><a class="nav-link" href="/Receipt/Index"><span class="nav-text">Receipts</span></a></li>
```

### 2. Add Chevron Rotation JavaScript

Add this script at the bottom of _Layout.cshtml (before the closing `</body>` tag, around line 197):

```html
<script>
    // Handle sidebar collapse chevron rotation and active states
    document.addEventListener('DOMContentLoaded', function () {
        // Chevron rotation on collapse toggle
        const collapseToggles = document.querySelectorAll('[data-bs-toggle="collapse"]');
        
        collapseToggles.forEach(function (toggle) {
            const target = toggle.getAttribute('data-bs-target');
            const collapseElement = document.querySelector(target);
            const icon = toggle.querySelector('.collapse-icon');
            
            if (collapseElement && icon) {
                // Set initial state
                if (collapseElement.classList.contains('show')) {
                    icon.classList.remove('fa-chevron-right');
                    icon.classList.add('fa-chevron-down');
                    toggle.classList.remove('collapsed');
                }
                
                // Listen to collapse events
                collapseElement.addEventListener('show.bs.collapse', function () {
                    icon.classList.remove('fa-chevron-right');
                    icon.classList.add('fa-chevron-down');
                    toggle.classList.remove('collapsed');
                });
                
                collapseElement.addEventListener('hide.bs.collapse', function () {
                    icon.classList.remove('fa-chevron-down');
                    icon.classList.add('fa-chevron-right');
                    toggle.classList.add('collapsed');
                });
            }
        });
        
        // Active link highlighting
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.sidebar-content .nav-link');
        
        navLinks.forEach(function (link) {
            const href = link.getAttribute('href');
            if (href && href !== '#' && currentPath.startsWith(href)) {
                link.classList.add('active');
                
                // Expand parent collapse if nested
                const parentCollapse = link.closest('.collapse');
                if (parentCollapse) {
                    parentCollapse.classList.add('show');
                    const parentToggle = document.querySelector(`[data-bs-target="#${parentCollapse.id}"]`);
                    if (parentToggle) {
                        parentToggle.classList.remove('collapsed');
                        const parentIcon = parentToggle.querySelector('.collapse-icon');
                        if (parentIcon) {
                            parentIcon.classList.remove('fa-chevron-right');
                            parentIcon.classList.add('fa-chevron-down');
                        }
                    }
                }
            }
        });
    });
</script>
```

### 3. Add CSS for Smooth Animations

Add this to `wwwroot/css/site.css`:

```css
/* Sidebar Collapse Animations */
.collapse-icon {
    transition: transform 0.3s ease-in-out;
}

.nav-link.collapsed .collapse-icon {
    transform: rotate(0deg);
}

.nav-link:not(.collapsed) .collapse-icon {
    transform: rotate(90deg);
}

/* Smooth collapse animation */
.collapse {
    transition: height 0.35s ease;
}

/* Active link highlighting */
.sidebar-content .nav-link.active {
    background-color: rgba(255, 255, 255, 0.1);
    color: #fff;
    font-weight: 600;
    border-left: 3px solid #0d6efd;
}

.sidebar-content .nav-link:hover {
    background-color: rgba(255, 255, 255, 0.05);
}

/* Nested menu styling */
.nav-link .ms-auto {
    margin-left: auto !important;
}

/* Improve chevron alignment */
.nav-link {
    display: flex;
    align-items: center;
    padding: 0.75rem 1rem;
}

.nav-icon {
    width: 20px;
    text-align: center;
    margin-right: 10px;
}

.nav-text {
    flex-grow: 1;
}
```

## Testing

After applying these fixes:
1. Open any page in the WMS
2. Click on "Settings / Users" - chevron should rotate smoothly
3. The current active page link should be highlighted
4. Collapse animations should be smooth
5. Links to Procurement screens should work

## Complete Updated Settings Section (Optional - Full Replacement)

If you prefer to replace the entire settings section (lines 138-154), use this:

```cshtml
<!-- Settings / Users -->
<li class="nav-item">
    <a class="nav-link collapsed" 
       href="#" 
       data-bs-toggle="collapse" 
       data-bs-target="#settingsCollapse" 
       aria-expanded="false" 
       aria-controls="settingsCollapse">
        <i class="fas fa-cog nav-icon"></i>
        <span class="nav-text">Settings / Users</span>
        <i class="fas fa-chevron-right ms-auto collapse-icon"></i>
    </a>
    <div class="collapse" id="settingsCollapse" data-bs-parent="#sidebarNav">
        <ul class="nav flex-column ms-3">
            <li class="nav-item">
                <a class="nav-link" href="/User/Index">
                    <span class="nav-text">User Management</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Role/Index">
                    <span class="nav-text">Roles & Permissions</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Settings/Index">
                    <span class="nav-text">System Settings</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Profile/Index">
                    <span class="nav-text">Profile</span>
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/Home/Privacy">
                    <span class="nav-text">Privacy</span>
                </a>
            </li>
        </ul>
    </div>
</li>
```

This version maintains proper Bootstrap markup, clean structure, and ARIA attributes for accessibility.
