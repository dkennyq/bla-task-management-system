//  MongoDB Initialization Script
//  This script runs when the MongoDB container is first created

print("🚀 Starting MongoDB initialization for tasksdb...");

// Switch to tasksdb database
db = db.getSiblingDB("tasksdb");

// Create collections
db.createCollection("tasks");

print("✅ Created tasks collection");

// Create indexes
db.tasks.createIndex({ userId: 1 });
db.tasks.createIndex({ status: 1 });
db.tasks.createIndex({ dueDate: 1 });
db.tasks.createIndex({ createdAt: -1 });

print("✅ Created indexes on tasks collection");

// Insert seed data
const seedTasks = [
  {
    id: UUID(),
    title: "Setup Development Environment",
    description:
      "Install all necessary tools and configure the development environment",
    status: "Completed",
    priority: "High",
    dueDate: new Date("2026-06-01"),
    userId: "00000000-0000-0000-0000-000000000001", // Will match seeded user
    createdAt: new Date("2026-05-25"),
    updatedAt: new Date("2026-06-01"),
  },
  {
    id: UUID(),
    title: "Implement User Authentication",
    description:
      "Create JWT-based authentication system with login and register endpoints",
    status: "InProgress",
    priority: "High",
    dueDate: new Date("2026-06-15"),
    userId: "00000000-0000-0000-0000-000000000001",
    createdAt: new Date("2026-06-05"),
    updatedAt: new Date("2026-06-09"),
  },
  {
    id: UUID(),
    title: "Create Task Management UI",
    description: "Build Vue.js components for task CRUD operations",
    status: "Pending",
    priority: "Medium",
    dueDate: new Date("2026-06-20"),
    userId: "00000000-0000-0000-0000-000000000001",
    createdAt: new Date("2026-06-08"),
    updatedAt: new Date("2026-06-08"),
  },
  {
    id: UUID(),
    title: "Write Unit Tests",
    description: "Achieve 80% code coverage with comprehensive unit tests",
    status: "Pending",
    priority: "High",
    dueDate: new Date("2026-06-25"),
    userId: "00000000-0000-0000-0000-000000000001",
    createdAt: new Date("2026-06-09"),
    updatedAt: new Date("2026-06-09"),
  },
  {
    id: UUID(),
    title: "Deploy to Production",
    description: "Configure CI/CD pipeline and deploy to cloud infrastructure",
    status: "Pending",
    priority: "Low",
    dueDate: new Date("2026-06-30"),
    userId: "00000000-0000-0000-0000-000000000001",
    createdAt: new Date("2026-06-09"),
    updatedAt: new Date("2026-06-09"),
  },
];

db.tasks.insertMany(seedTasks);

print("✅ Inserted " + seedTasks.length + " seed tasks");
print("🎉 MongoDB initialization completed successfully!");
