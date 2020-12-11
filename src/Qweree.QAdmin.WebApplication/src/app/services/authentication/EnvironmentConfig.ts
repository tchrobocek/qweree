export interface EnvironmentConfig {
  readonly authentication: AuthenticationConfig;
}

export interface AuthenticationConfig {
  readonly baseUri: string;
}
