import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Symptom } from '../models/symptom.model';

@Injectable({
  providedIn: 'root'
})
export class SymptomService {
  constructor(private api: ApiService) {}

  getSymptoms(): Observable<Symptom[]> {
    return this.api.get<Symptom[]>('symptoms');
  }

  getSymptom(id: string): Observable<Symptom> {
    return this.api.get<Symptom>(`symptoms/${id}`);
  }

  createSymptom(symptom: Symptom): Observable<Symptom> {
    return this.api.post<Symptom>('symptoms', symptom);
  }

  updateSymptom(id: string, symptom: Symptom): Observable<Symptom> {
    return this.api.put<Symptom>(`symptoms/${id}`, symptom);
  }

  deleteSymptom(id: string): Observable<void> {
    return this.api.delete<void>(`symptoms/${id}`);
  }
}