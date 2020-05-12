import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';

/**
 * Base for services talking to the FleetPing API.
 */
export abstract class ApiService {
  protected readonly baseUrl = environment.apiUrl;

  protected constructor(protected http: HttpClient) { }

  protected url(path: string): string {
    return `${this.baseUrl}/${path.replace(/^\//, '')}`;
  }
}
