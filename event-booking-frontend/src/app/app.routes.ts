import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { LoginComponent } from './components/auth/login/login.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { AuthGuard } from './guards/auth.guard';
import { SendPinComponent } from './components/auth/send-pin/send-pin.component';
import { EnterPinComponent } from './components/auth/enter-pin/enter-pin.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { EventDetailsComponent } from './components/event-details/event-details.component';
import { AdminPanelComponent } from './components/admin-panel/admin-panel.component';
import { adminGuard } from './guards/admin.guard';



export const routes: Routes = [
    {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'home', component:HomeComponent, title:'Home Page'}, 
  { path: 'events/:id', component: EventDetailsComponent, title: 'Event Details Page' },
  {path:'register', component:RegisterComponent, title:'Register Page'},
  {path:'login', component:LoginComponent, title:'Login Page'},
  {path:'changepassword', canActivate:[AuthGuard], component:ChangePasswordComponent, title:'Change Password Page'},
  
  {path:'sendpin', component:SendPinComponent, title:'Send Pin Page'},
 { path: 'admin-panel', component: AdminPanelComponent, canActivate: [adminGuard], title: 'Admin Panel' },
  {path:'enterpin/:email/:expireAt', component:EnterPinComponent, title:'Enter Pin Page'},
  
  {path:'resetpassword/:email', component:ResetPasswordComponent, title:'Forget Password Page'},
];
