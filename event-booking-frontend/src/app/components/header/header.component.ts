import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-header',
  imports: [CommonModule,RouterLink,RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  darkMode = false;
  isLogin = false;
  showDropdown = false;
  userName: string = '';
  photo: string = '';
  constructor(private _AuthService : AuthService, private _CookieService: CookieService) {
      const theme = this._CookieService.get('theme');
      this.darkMode = theme === 'dark';
    }
  ngOnInit() {

  // const theme = this._CookieService.get('theme');
  //   this.darkMode = theme === 'dark';
  //   this.updateBodyClass();

  this._AuthService.themeSubject.subscribe(theme => {
    this.darkMode = theme === 'dark';
    this.updateBodyClass();
  });

    this._AuthService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          this.userName = data.displayName;
          this.photo = data.image;
        }
        else {
          this.isLogin = false;
        }
      }
    });
  }
  // toggleDarkMode() {
  //   this.darkMode = !this.darkMode;
  //   document.body.classList.toggle('dark-mode', this.darkMode);
    
  //   const modeToggle = document.querySelector('.mode-toggle');
  //   if (modeToggle) {
  //     modeToggle.classList.toggle('fa-moon', !this.darkMode);
  //     modeToggle.classList.toggle('fa-sun', this.darkMode);
  //   }
  // }
  toggleDarkMode() {
    this.darkMode = !this.darkMode;
    const newTheme = this.darkMode ? 'dark' : 'light';
    this._AuthService.themeSubject.next(newTheme);
    this._CookieService.set('theme', newTheme, {
      expires: 30,
      path: '/',
    });
    this.updateBodyClass();
  }
  updateBodyClass() {
    if (typeof window !== 'undefined' && typeof document !== 'undefined') {
      if (this.darkMode) {
        document.body.classList.add('dark-mode');
      } else {
        document.body.classList.remove('dark-mode');
      }
    }
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  logout() {
    this._AuthService.signout();
  }
}
