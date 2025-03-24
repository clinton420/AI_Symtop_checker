export interface DashboardStats {
  totalPredictions: number;
  todayPredictions: number;
  highUrgencyCount: number;
  uniqueSymptomsCount: number;
  averageConfidence: number;
  generatedAt: Date;
}

export interface PaginationInfo {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}