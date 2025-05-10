import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  darkMode = false;
  isLoggedIn = false;
  showDropdown = false;
  ngOnInit() {
    this.isLoggedIn = !!localStorage.getItem('token'); 
  }
  toggleDarkMode() {
    this.darkMode = !this.darkMode;
    document.body.classList.toggle('dark-mode', this.darkMode);
    
    const modeToggle = document.querySelector('.mode-toggle');
    if (modeToggle) {
      modeToggle.classList.toggle('fa-moon', !this.darkMode);
      modeToggle.classList.toggle('fa-sun', this.darkMode);
    }
  }

  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  logout() {
    localStorage.removeItem('token');
    this.isLoggedIn = false;
    this.showDropdown = false; 
    window.location.href = '/login';
  }
}
