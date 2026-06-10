import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import tseslint from 'typescript-eslint'

export default tseslint.config(
  // Ignore patterns
  {
    ignores: [
      '**/dist/**',
      '**/node_modules/**',
      '**/coverage/**',
      '**/.vite/**',
      '*.config.js',
      '*.config.ts',
    ],
  },
  
  // Base JavaScript rules
  js.configs.recommended,
  
  // TypeScript rules (without strict type checking)
  ...tseslint.configs.recommended,
  
  // Vue rules
  ...pluginVue.configs['flat/recommended'],
  
  // Custom configuration for source files
  {
    files: ['src/**/*.{ts,vue}'],
    languageOptions: {
      ecmaVersion: 'latest',
      sourceType: 'module',
      parserOptions: {
        parser: tseslint.parser,
        project: './tsconfig.app.json',
        extraFileExtensions: ['.vue'],
      },
    },
    rules: {
      // Vue-specific rules (relaxed for better DX)
      'vue/multi-word-component-names': 'off',
      'vue/no-v-html': 'warn',
      'vue/max-attributes-per-line': 'off',
      'vue/singleline-html-element-content-newline': 'off',
      'vue/html-self-closing': ['warn', {
        html: { void: 'never', normal: 'never', component: 'always' },
        svg: 'always',
        math: 'always',
      }],
      'vue/attributes-order': 'off',
      
      // TypeScript rules
      '@typescript-eslint/no-explicit-any': 'warn',
      '@typescript-eslint/no-unused-vars': ['warn', { 
        argsIgnorePattern: '^_',
        varsIgnorePattern: '^_'
      }],
      
      // General rules
      'no-console': ['warn', { allow: ['warn', 'error'] }],
      'prefer-const': 'warn',
    },
  },
  
  // Test files configuration
  {
    files: ['**/*.spec.ts', '**/*.test.ts'],
    rules: {
      '@typescript-eslint/no-explicit-any': 'off',
    },
  },
  
  // Config files (no TypeScript type checking)
  {
    files: ['*.config.{js,ts}'],
    languageOptions: {
      ecmaVersion: 'latest',
      sourceType: 'module',
    },
    rules: {
      '@typescript-eslint/no-explicit-any': 'off',
    },
  }
)
