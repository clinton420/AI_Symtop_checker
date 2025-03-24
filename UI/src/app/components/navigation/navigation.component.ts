import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container">
        <a class="navbar-brand d-flex align-items-center" routerLink="/">
          <div class="rotating-cube me-2"></div>
          Symptom Checker
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <a class="nav-link" routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/symptoms" routerLinkActive="active">Symptoms</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/predictions" routerLinkActive="active">Predictions</a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .rotating-cube {
      width: 30px;
      height: 30px;
      background: linear-gradient(45deg, #ffffff, #f0f0f0);
      animation: rotate 4s infinite linear;
      transform-style: preserve-3d;
      position: relative;
    }

    @keyframes rotate {
      from { transform: rotateX(0deg) rotateY(0deg); }
      to { transform: rotateX(360deg) rotateY(360deg); }
    }

    .navbar {
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
  `]
})
export class NavigationComponent {}