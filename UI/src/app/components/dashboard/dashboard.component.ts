import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardStats } from '../../models/dashboard.model';
import { PredictionListComponent } from '../prediction-list/prediction-list.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PredictionListComponent],
  template: `
    <div class="dashboard-container">
      <div class="floating-cube"></div>
      <h1 class="display-4 mb-4">Dashboard</h1>
      
      <div class="row g-4 mb-5">
        <div class="col-md-3">
          <div class="stat-card">
            <div class="stat-icon">🔍</div>
            <h3>Total Predictions</h3>
            <p class="stat-number">{{stats.totalPredictions}}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card">
            <div class="stat-icon">📊</div>
            <h3>Today's Checks</h3>
            <p class="stat-number">{{stats.todayPredictions}}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card">
            <div class="stat-icon">⚡</div>
            <h3>High Urgency</h3>
            <p class="stat-number">{{stats.highUrgencyCount}}</p>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card">
            <div class="stat-icon">📈</div>
            <h3>Confidence</h3>
            <p class="stat-number">{{stats.averageConfidence}}%</p>
          </div>
        </div>
      </div>

      <app-prediction-list></app-prediction-list>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 2rem;
      position: relative;
      overflow: hidden;
    }

    .floating-cube {
      position: absolute;
      top: 20px;
      right: 20px;
      width: 100px;
      height: 100px;
      background: linear-gradient(45deg, #6e8efb, #a777e3);
      animation: float 6s ease-in-out infinite;
      transform-style: preserve-3d;
      box-shadow: 0 0 20px rgba(0,0,0,0.1);
    }

    @keyframes float {
      0% { transform: translateY(0) rotate(0deg); }
      50% { transform: translateY(-20px) rotate(180deg); }
      100% { transform: translateY(0) rotate(360deg); }
    }

    .stat-card {
      background: white;
      border-radius: 15px;
      padding: 1.5rem;
      box-shadow: 0 4px 20px rgba(0,0,0,0.05);
      transition: transform 0.3s ease;
    }

    .stat-card:hover {
      transform: translateY(-5px);
    }

    .stat-icon {
      font-size: 2rem;
      margin-bottom: 1rem;
    }

    .stat-number {
      font-size: 2rem;
      font-weight: bold;
      color: #4a69bd;
      margin: 0;
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats = {
    totalPredictions: 0,
    todayPredictions: 0,
    highUrgencyCount: 0,
    uniqueSymptomsCount: 0,
    averageConfidence: 0,
    generatedAt: new Date()
  };

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.loadDashboardStats();
  }

  loadDashboardStats() {
    this.dashboardService.getStats().subscribe(
      (stats) => {
        this.stats = stats;
      },
      (error) => {
        console.error('Error loading dashboard stats:', error);
      }
    );
  }
}