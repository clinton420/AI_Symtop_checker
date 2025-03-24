import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { UserInterfaceComponent } from './components/user-interface/user-interface.component';
import { AdminInterfaceComponent } from './components/admin-interface/admin-interface.component';
import { AuthGuard } from './guards/auth.guard';

@NgModule({
  imports: [
    BrowserModule,
    AppComponent,
    LoginComponent,
    UserInterfaceComponent,
    AdminInterfaceComponent,
    RouterModule.forRoot([
      { path: '', redirectTo: '/login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'user', component: UserInterfaceComponent, canActivate: [AuthGuard] },
      { path: 'admin', component: AdminInterfaceComponent, canActivate: [AuthGuard] }
    ])
  ],
  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }