import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/auth/login/login.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
    {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'home', component:HomeComponent, title:'Home Page'},
  {path:'login', component:LoginComponent, title:'Login Page'},
//   {path:'changepassword', component:ChangePasswordComponent, title:'Change Password Page'},
  {path:'changepassword', canActivate:[AuthGuard], component:ChangePasswordComponent, title:'Change Password Page'},
];
