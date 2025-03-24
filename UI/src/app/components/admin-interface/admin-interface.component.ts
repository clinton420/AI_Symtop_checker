import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-interface',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="admin-container">
      <header>
        <h1>Admin Dashboard</h1>
        <nav>
          <button (click)="logout()">Logout</button>
        </nav>
      </header>
      
      <main>
        <div class="dashboard">
          <div class="stats-grid">
            <div class="stat-card">
              <h3>Total Checks</h3>
              <p class="stat-number">1,245</p>
            </div>
            <div class="stat-card">
              <h3>Today's Checks</h3>
              <p class="stat-number">124</p>
            </div>
            <div class="stat-card">
              <h3>Accuracy Rate</h3>
              <p class="stat-number">85%</p>
            </div>
            <div class="stat-card">
              <h3>Active Users</h3>
              <p class="stat-number">324</p>
            </div>
          </div>

          <div class="recent-activity">
            <h2>Recent Activity</h2>
            <table>
              <thead>
                <tr>
                  <th>Date</th>
                  <th>User</th>
                  <th>Symptoms</th>
                  <th>Diagnosis</th>
                  <th>Confidence</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let activity of recentActivity">
                  <td>{{ activity.date }}</td>
                  <td>{{ activity.user }}</td>
                  <td>{{ activity.symptoms }}</td>
                  <td>{{ activity.diagnosis }}</td>
                  <td>{{ activity.confidence }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .admin-container {
      min-height: 100vh;
      background: #f8f9fa;
    }
    header {
      background: #4a69bd;
      color: white;
      padding: 1rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    main {
      padding: 2rem;
    }
    .dashboard {
      max-width: 1200px;
      margin: 0 auto;
    }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
      margin-bottom: 2rem;
    }
    .stat-card {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .stat-number {
      font-size: 2rem;
      color: #4a69bd;
      margin: 0;
    }
    .recent-activity {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 1rem;
    }
    th, td {
      padding: 1rem;
      text-align: left;
      border-bottom: 1px solid #ddd;
    }
    th {
      background: #f8f9fa;
      font-weight: 600;
    }
  `]
})
export class AdminInterfaceComponent {
  recentActivity = [
    {
      date: '2025-02-27',
      user: 'user@example.com',
      symptoms: 'Headache, Fever',
      diagnosis: 'Common Cold',
      confidence: '85%'
    },
    {
      date: '2025-02-27',
      user: 'another@example.com',
      symptoms: 'Cough, Fatigue',
      diagnosis: 'Viral Infection',
      confidence: '75%'
    }
  ];

  logout() {
    // Implement logout logic
  }
}