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

// Helper to generate a GUID string (RFC 4122 v4)
function guid() {
  const hex = '0123456789abcdef';
  let s = '';
  for (let i = 0; i < 36; i++) {
    if (i === 8 || i === 13 || i === 18 || i === 23) {
      s += '-';
    } else if (i === 14) {
      s += '4';
    } else if (i === 19) {
      s += hex[(Math.random() * 4 | 0) + 8];
    } else {
      s += hex[Math.random() * 16 | 0];
    }
  }
  return s;
}

// Insert seed data
const seedTasks = [
  {
    _id: guid(),
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
    _id: guid(),
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
    _id: guid(),
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
    _id: guid(),
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
    _id: guid(),
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
