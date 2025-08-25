import { Component, ChangeDetectionStrategy, signal, inject } from "@angular/core";
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import { MatIcon } from "@angular/material/icon";
import { AuthService } from "../../../core/services/auth.service";

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, MatIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  sidebarOpen = signal(false);
  profileMenuOpen = signal(false);
  user = this.authService.user;
  settingsDropdownOpen = signal(false);

  
  toggleSidebar() {
    this.sidebarOpen.update(value => !value);
  }

  closeSidebar() {
    this.sidebarOpen.set(false);
    this.settingsDropdownOpen.set(false);
  }
  toggleSettingsDropdown() {
    this.settingsDropdownOpen.update(open => !open);
  }
  toggleProfileMenu() {
    this.profileMenuOpen.update(open => !open);
  }

  closeProfileMenu() {
    this.profileMenuOpen.set(false);
  }

  logout() {
    this.authService.logout();
  }

  isSettingsRoute(): boolean {
  const currentUrl = this.router.url;
  return currentUrl.includes('/admin/settings') || 
         currentUrl.includes('/admin/website-management/cases') || 
         currentUrl.includes('/admin/website-management/work-types');
}

  
}